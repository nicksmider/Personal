#include <stdio.h>
#include <stdlib.h>


int main(int argc, char *argv[]){

	FILE *file;
	char *found_string = (char *)malloc(sizeof(char) * 5);;
	char current = '\0';
	int size_counter = 0;
	int holder = 0;

	if (argc != 2){
		if (argc > 2){
			printf("Please only submit one file when running the program.\n");
		}
		else {
			printf("Please submit a file to read from as a command line arguement.\n");
		}
		return -1;
	}

	file = fopen(argv[1], "rb");

	if (file == 0){
		printf("The file could not be read.\n");
		return -1;
	}

	while (!feof(file)){
		fread(&current, sizeof(char), 1, file);
		

		if ((current >= 32 && current <= 126) || current == 9){
			size_counter++;
		}
		else {
			if (size_counter >= 4){
				int neg_size_counter = (size_counter * -1) - 1;
				fseek(file, neg_size_counter, SEEK_CUR); 
				found_string = realloc(found_string, sizeof(char) * (size_counter + 1));
				fread(found_string, sizeof(char), size_counter, file);
				found_string[size_counter] = '\0';
				printf("%s\n", found_string);
			}
			size_counter = 0;
		}
	}

	return 0;
}