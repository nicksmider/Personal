//Nicholas Smider

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <unistd.h>
#include <pthread.h>
#include <time.h>

#define four_kb 4096 //Size of 4KB
#define read_in 1
#define SIZE 256 //For time 

static pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

/*To pass port and ip info along with client fd connection*/
struct client_info {
    int sin_port;     
    char *sin_ip;     
    int fd; 
};

//Handles each request to the server
void *handle_request(void *new_client_void);

int main(){

	/*Make the server socket */
	int server_fd, connection_fd, addr_len;
	struct sockaddr_in addr;

	server_fd = socket(PF_INET, SOCK_STREAM, IPPROTO_IP);

	if (server_fd == -1){
		printf("\nSocket failed.\n");
		return 1;
	}

	memset(&addr, 0, sizeof(addr));
	addr.sin_family = AF_INET;
	addr.sin_port = htons(50890);
	addr.sin_addr.s_addr = INADDR_ANY;

	if (bind(server_fd, (struct sockaddr *)&addr, sizeof(addr)) < 0){
		perror("\nBinding Unsuccessful\n");
		return 1;
	}
	
	/*Start listening for connections*/
	listen(server_fd, 10);

	addr_len = sizeof(addr);

	pthread_t new_thread_id;

	/*accept blocks if there is nothing waiting, so we can use
		it as an argument for the while loop. will not go until connection 
		is accepted*/
	while (connection_fd = accept(server_fd, (struct sockaddr *)&addr, (socklen_t*)&addr_len)){
		printf("Connection found");
		char *ip_buffer;

		/*Package info to send to handler*/
		struct client_info *new_client = malloc(sizeof(struct client_info));
		new_client->sin_port = ntohs(addr.sin_port);
		/*to my understanding inet_ntoa gets overwritten when a new thing comes
			so I copy it incase it is overwritten*/
		ip_buffer = inet_ntoa(addr.sin_addr);
		char *copy_ip_buffer = malloc(strlen(ip_buffer));
		strcpy(copy_ip_buffer, ip_buffer);
		new_client->sin_ip = copy_ip_buffer;
		new_client->fd = connection_fd;

		//Create thread with connection, perror if it fails
		if (pthread_create(&new_thread_id, NULL, handle_request, (void *)new_client) < 0){
			perror("\nError making pthread.\n");
			return 1;
		}

	}
	close(server_fd);
	return 0;
}

//Handles each request to the server
void *handle_request(void *new_client_void){
	/*Unpackages info from parameter*/
	struct client_info *new_client = (struct client_info *)new_client_void;
	int connection_fd = new_client->fd;
	char *ip_buffer = new_client->sin_ip;
	int connection_port = new_client->sin_port;
	///////////////////////////////////////

	char client_in_message[four_kb]; //buffer for GET message 
	int spot = 0; //spot in client_in_message for copying
	char temp_in; //holds current character being checked
	int clrf = 0; //count for clrf 

	/*while we haven't found 2 clrf and we haven't taken in 4KB yet
		read in character by character*/
	while (clrf <= 1 && spot < four_kb){
		if (recv(connection_fd, &temp_in, read_in, 0) > 0){
			//Got character

			//If first part of CLRF, check next character
			if (temp_in == '\r'){
				client_in_message[spot] = temp_in; //store \r
				if (recv(connection_fd, &temp_in, read_in, MSG_PEEK) > 0){
					//next char
					if (temp_in == '\n'){
						//clrf found, take in the \n to buffer and increment counter
						spot++;
						recv(connection_fd, &temp_in, read_in, 0);
						client_in_message[spot] = temp_in;
						clrf++;
					}

				}
			}
			else{
				//regular character, read in
				client_in_message[spot] = temp_in;
			}
			spot++; //increment spot for next character
		}
	}

	client_in_message[spot] = '\0'; //Make sure the end is null terminated

	//Check if it is a GET message, if not send 404 message
	if (strncmp(client_in_message, "GET /", 5) == 0){
		
		/*Read name of file trying to be accessed
			located at client_in_message spot 5 to start*/
		char _file[20];
		int i;
		for (i = 0; client_in_message[i + 5] != ' '; i++){
			_file[i] = client_in_message[i + 5];
		}
		_file[i] = '\0';//Make sure the end is null terminated

		//Attempt to pen file 
		FILE *file = fopen(_file, "r");
		if (file != NULL){
			//File is found

			//Send Ok Message 
			char *ok = "\nHTTP/1.1 200 OK\n";
			send(connection_fd, ok, strlen(ok), 0);

			//Get Date and Send

			char time_buffer[SIZE];
  			time_t curtime;
  			struct tm *loctime;

  			/* Get current time */ 
  			curtime = time(NULL);
  			loctime = localtime(&curtime);

  			/*Format and send date info*/
  			strftime(time_buffer, SIZE, "Date: %a, %d %b %Y %X %Z\n", loctime);
  			send(connection_fd, time_buffer, strlen(time_buffer), 0);

  			/*Get content length and send */
  			int file_size;
			fseek(file, 0, SEEK_END); 
			file_size = ftell(file); 
			fseek(file, 0, SEEK_SET);

			char content[25];
			strcpy(content, "Content-Length: ");
			char number[9]; // 25 - 16 = 9 = rest of content
			sprintf(number, "%d", number);
			strcat(content, number);

			send(connection_fd, content, strlen(content), 0);

			//Send constant fields 
			char close_type[75];
			strcpy(close_type, "\nConnection : close\nContent-Type: ");
			char file_name[20];
			for (i = 0; _file[i] != '.'; i++){
				file_name[i] = _file[i];
			}
			file_name[i] = '\0';
			strcat(close_type, file_name);
			strcat(close_type, "/html\n\n");
			send(connection_fd, close_type, strlen(close_type), 0);

			//Send contents of File
			char *file_contents = (char *)malloc((file_size+1)*sizeof(char));
			fread(file_contents, 1, file_size, file);
			send(connection_fd, file_contents, strlen(file_contents), 0);


			//Write to stats.txt
			pthread_mutex_lock(&mutex);
			FILE *stats = fopen("stats.txt", "a+"); //creates file if doesn't exist
			fwrite(client_in_message, 1, strlen(client_in_message), stats);
			char client_out[50];
			strcpy(client_out, "Client: ");
			strcat(client_out, ip_buffer);
			char port_out[10];
			sprintf(port_out, "%d", connection_port);
			strcat(client_out, ":");
			strcat(client_out, port_out);
			strcat(client_out, "\n\n");
			fwrite(client_out, 1, strlen(client_out), stats);
			fclose(stats);
			pthread_mutex_unlock(&mutex);

		}
		else {
			//File not found
			char *not = "\nHTTP/1.1 404 Not Found\n";
			send(connection_fd, not, strlen(not), 0);
		}
		
	}
	else {
		//Not a GET message
		char *not = "\nNot a proper GET request\n";
		send(connection_fd, not, strlen(not), 0);
	}

	//Free Both malloced arguements
	free(ip_buffer);
	free(new_client);
	close(connection_fd);
	return 0;

}
