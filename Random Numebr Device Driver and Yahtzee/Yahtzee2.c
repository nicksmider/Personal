#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <unistd.h>
#include <fcntl.h>

#define ones_value 0
#define twos_value 1
#define threes_value 2
#define fours_value 3
#define fives_value 4
#define sixes_value 5
#define three_kind_value 6
#define four_kind_value 7
#define small_straight_value 8
#define large_straight_value 9
#define full_house_value 10
#define yahtzee_value 11
#define chance_value 12

#define upper_adjustment 1
#define lower_adjustment -5

#define fill 1
#define clear 0

void fill_section_info();
void roll_dice();
void display_dice();
int which_reroll(int count);
void ask_upper_or_lower();
void ask_which_section(int selection);
int get_value(int spot);
int totalscores();
void print_score_card();
void free_names();

struct Section{
	unsigned char filled;
	int value;
	char *name;
};

const int select_upper_section = 1;
const int select_lower_section = 2;
int upper_section_bonus = 0;
struct Section sections[13];
int dice[5];
int sorted_dice[5];


int main(){
	srand(time(NULL));
	fill_section_info();

	int reroll_count;
	int continue_playing;
	int turns = 0;

	while (continue_playing){
		continue_playing = 1;
		reroll_count = 0;
		////Rolling Dice for turn//////
		roll_dice();
		printf("\nYour roll: \n");
		display_dice();

		//Rerolling
		while (reroll_count < 2){
			//printf("\nReroll count before: %d\n", reroll_count);
			int end = which_reroll(reroll_count);
			//printf("\nReroll count after: %d\n", reroll_count);
			if (reroll_count == 0 && end != 3){
				printf("\nYour second roll: \n");
				display_dice();
			}
			else if (reroll_count == 1 && end != 3){
				printf("\nYour third and final roll: \n");
				display_dice();
				break;
			}

			if (end){
				break;
			}

			reroll_count++;
		}


		////////////////////////////////////////
		/*Dice Finalized for turn*/
		////////////////////////////////////////
		int user_input = -1;
		int adjust_selection = -1;
		int not_filled = 1;

		ask_upper_or_lower();
		//Get user input for whether they want to fill Upper or Lower Sections
		while (user_input != select_upper_section && user_input != select_lower_section){
			scanf("%d", &user_input);
			//Set adjustment for later selection
			if (user_input == (select_upper_section)){
				adjust_selection = upper_adjustment;
			}
			else if (user_input == (select_lower_section)){
				adjust_selection = lower_adjustment;
			}
			else {
				printf("\nPlease enter a valid option. ");
			}
		}
		///////////////////////////////
		//Ask which specific section
		ask_which_section(user_input);
		user_input = 0;
		while (user_input >= ones_value && user_input <= chance_value){
			not_filled = 1;
			scanf("%d", &user_input);
			//printf("\nAdjustment %d - %d\n", user_input, adjust_selection);
			user_input -= adjust_selection;

			//Set adjustment for later selection
			if (user_input >= ones_value && user_input <= chance_value){
				if (sections[user_input].filled){
					printf("\n%s is already used.\n", sections[user_input].name);
					not_filled = 0;
				}

				if (not_filled){
					sections[user_input].filled = 1;
					sections[user_input].value = get_value(user_input);
					break;
				}
			}
			printf("\nPlease enter a valid option. ");
		}
		////////////////////////////////
		//If we get here they have entered a valid option

		//print current status of player card
		turns++;
		if (turns == 13){
			//game is over
			printf("\nFinal Score Card:\n");
			printf("Your final score is: %d\n",totalscores());
			continue_playing = 0;
		}
		else {
			printf("\n\nYour score so far is: %d\n", totalscores());
		}

		print_score_card();

	}

	free_names();
	return 0;
}

int roll_die(){
	int result;
	int open_int = open("/dev/dice_driver", O_RDONLY);//(rand()%(6)) + 1;
	int read_int = read(open_int, &result, 1);
	close(open_int);
	return result;
}

//when testing without device driver
int roll_die2(){
	return (rand() % 6) + 1;
}

void roll_dice(){
	int i;
	for (i = 0; i < 5; i++){
		dice[i] = roll_die2();
	}
}

void reroll_die(int die){
	dice[die] = roll_die2();
}

int which_reroll(int count){
	printf("\nWhich dice to reroll? ");

	char input[11];

	fgets(input, sizeof(input), stdin);
	if (input[0] == '\n'){
		fgets(input, sizeof(input), stdin);
	}


	if (strchr(input, '0')){
		return 3;
	}

	int i;
	for (i = 1; i < 6; i++){
		char a = i + '0';

		if (strchr(input, a)){
			dice[i - 1] = roll_die2();
		}
	}

	return count;
}

void display_dice(){
	printf("\n%d %d %d %d %d\n", dice[0], dice[1], dice[2], dice[3], dice[4]);
}

void display_copydice(){
	printf("\n%d %d %d %d %d\n", sorted_dice[0], sorted_dice[1], sorted_dice[2], sorted_dice[3], sorted_dice[4]);
}

void ask_upper_or_lower(){
	printf("\nPlace dice into:\n1) Upper Section\n2) Lower Section\nSelection? ");
}

void ask_which_section(int selection){
	printf("\nPlace dice into:\n");

	if (selection == select_lower_section){
		printf("1) Three of a Kind\n");
		printf("2) Four of a Kind\n");
		printf("3) Small Straight\n");
		printf("4) Large Straight\n");
		printf("5) Full House\n");
		printf("6) Yahtzee\n");
		printf("7) Chance\n");
	}
	else {
		printf("1) Ones\n");
		printf("2) Twos\n");
		printf("3) Threes\n");
		printf("4) Fours\n");
		printf("5) Fives\n");
		printf("6) Sixes\n");
	}

	printf("\nSelection? ");
}

void free_names(){
	int i;
	for (i = ones_value; i <= chance_value; i++){
		free(sections[i].name);
	}
}

void fill_section_info(){

	//Set origional values
	int i;
	for (i = ones_value; i <= chance_value; i++){
		sections[i].filled = 0;
		//sections[i].value = NULL;
	}


	//////////////////////////////////
	sections[ones_value].name = (char *)malloc((sizeof(char) * 4) + 1);
	strcpy(sections[ones_value].name, "Ones");

	sections[twos_value].name = (char *)malloc((sizeof(char) * 4) + 1);
	strcpy(sections[twos_value].name, "Twos");

	sections[threes_value].name = (char *)malloc((sizeof(char) * 6) + 1);
	strcpy(sections[threes_value].name, "Threes");

	sections[fours_value].name = (char *)malloc((sizeof(char) * 5) + 1);
	strcpy(sections[fours_value].name, "Fours");

	sections[fives_value].name = (char *)malloc((sizeof(char) * 5) + 1);
	strcpy(sections[fives_value].name, "Fives");

	sections[sixes_value].name = (char *)malloc((sizeof(char) * 5) + 1);
	strcpy(sections[sixes_value].name, "Sixes");

	/////////////////////////////////////
	sections[three_kind_value].name = (char *)malloc((sizeof(char) * 15) + 1);
	strcpy(sections[three_kind_value].name, "Three of a Kind");

	sections[four_kind_value].name = (char *)malloc((sizeof(char) * 14) + 1);
	strcpy(sections[four_kind_value].name, "Four of a Kind");

	sections[small_straight_value].name = (char *)malloc((sizeof(char) * 14) + 1);
	strcpy(sections[small_straight_value].name, "Small Straight");

	sections[large_straight_value].name = (char *)malloc((sizeof(char) * 14) + 1);
	strcpy(sections[large_straight_value].name, "Large Straight");

	sections[full_house_value].name = (char *)malloc((sizeof(char) * 10) + 1);
	strcpy(sections[full_house_value].name, "Full House");

	sections[yahtzee_value].name = (char *)malloc((sizeof(char) * 7) + 1);
	strcpy(sections[yahtzee_value].name, "Yahtzee");

	sections[chance_value].name = (char *)malloc((sizeof(char) * 6) + 1);
	strcpy(sections[chance_value].name, "Chance");
}

int compare_dice(const void* a, const void* b){
	int _a = *((int*)a);
	int _b = *((int*)b);
	return _a - _b;
}

void copy_dice(){
	int i;
	for (i = 0; i < 5; i++){
		sorted_dice[i] = dice[i];
	}
	qsort(sorted_dice, 5, sizeof(int), compare_dice);
}

int get_kind(int how_many){
	int consecutive = 0;
	int i;
	int last_value = sorted_dice[0];
	int total = sorted_dice[0];
	for (i = 1; i < 5; i++){
		total += sorted_dice[i];
		if (sorted_dice[i] == last_value){
			consecutive++;
		}
		else {
			consecutive = 0;
		}

		if (consecutive == (how_many - 1)){
			int j;
			for (j = (i +1); j < 5; j++){
				total += sorted_dice[j];
			}
			return total;
		}
		last_value = sorted_dice[i];
	}
	return 0;
}

int get_straight(int how_many){
	int consecutive = 0;
	int i;
	int last_value = sorted_dice[0];

	for (i = 1;  i < 5; i++){
		if (sorted_dice[i] == (last_value + 1)){
			consecutive++;
			last_value = sorted_dice[i];
		}
		else if (sorted_dice[i] != last_value){
			consecutive = 0;
		}

		if (consecutive == (how_many - 1)){
			if (how_many == 4){
				return 30;
			}
			if (how_many == 5){
				return 40;
			}
		}

	}
	return 0;
}

int get_full_house(){
	if (sorted_dice[0] == sorted_dice[1] && sorted_dice[0] != sorted_dice[2]){
		if (sorted_dice[2] == sorted_dice[3] && sorted_dice[2] == sorted_dice[4]){
			return 25;
		}
	}
	if (sorted_dice[0] == sorted_dice[1] && sorted_dice[0] == sorted_dice[2] && sorted_dice[0] != sorted_dice[3]){
		if (sorted_dice[3] == sorted_dice[4]){
			return 25;
		}
	}
	return 0;
}

int get_yahtzee(){
	if (dice[0] == dice[1] && dice[0] == dice[2] && dice[0] == dice[3] && dice[0] == dice[4]){
		return 50;
	}
	return 0;
}

int get_value(int spot){
	copy_dice();
	int i;
	int amount = 0;
	if (spot >= ones_value && spot <= sixes_value){
		for (i = 0; i < 5; i++){
			if (sorted_dice[i] == (spot + 1)){
				amount+= (spot + 1);
			}
		}
		return amount;
	}
	else if (spot == chance_value){
		for (i = 0; i < 6; i++){
			amount += sorted_dice[i];
		}
		return amount;
	}
	else if (spot == three_kind_value){
		return get_kind(3);
	}
	else if (spot == four_kind_value){
		return get_kind(4);
	}
	else if (spot == small_straight_value){
		return get_straight(4);
	}
	else if (spot == large_straight_value){
		return get_straight(5);
	}
	else if (spot == full_house_value){
		return get_full_house();
	}
	else if (spot == yahtzee_value){
		return get_yahtzee();
	}
	return -1;
}

int totalscores(){
	const int upper_bonus_needed = 63;
	const int upper_section_bonus_value = 35;
	int upper_score = 0;
	int total_score = 0;
	int i;
	for (i = ones_value; i <= sixes_value; i++){
		upper_score += sections[i].value;
	}
	total_score = upper_score;

	if (total_score >= upper_bonus_needed){
		upper_section_bonus = upper_section_bonus_value;
	}

	for (i = three_kind_value; i <= chance_value; i++){
		total_score += sections[i].value;
	}

	return total_score;
}

void print_score_card(){
	printf("\n%s%d%s%d", "Ones: " , sections[ones_value].value, " Fours: " , sections[fours_value].value);
	printf("\n%s%d%s%d", "Twos: ", sections[twos_value].value, " Fives: ", sections[fives_value].value);
	printf("\n%s%d%s%d", "Threes: ", sections[threes_value].value, " Sixes: ", sections[sixes_value].value);
	printf("\nUpper Section Bonus: %d", upper_section_bonus);
	printf("\n%s%d%s%d", "Three of a Kind: " , sections[three_kind_value].value, " Four of a Kind: " , sections[four_kind_value].value);
	printf("\n%s%d%s%d", "Small Straight: ", sections[small_straight_value].value, " Large Straight: ", sections[large_straight_value].value);
	printf("\n%s%d%s%d", "Full House: ", sections[full_house_value].value, " Yahtzee: ", sections[yahtzee_value].value);
	printf("\nChance: %d\n", sections[chance_value].value);
}
