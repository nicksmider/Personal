#include <stdio.h>
#include <string.h>
#include <stdlib.h>

//Written by Nicholas Smider

/*Stores header information. Read into all at once.*/
struct Header {
	unsigned short start_marker;
	unsigned short app1_marker;
	unsigned short app1_block;
	char exif_string[4];
	unsigned short NUL_terminator_zero_byte;
	char endian_type[2];
	unsigned short version_number;
	unsigned int offset;
	unsigned short TIFF_count;
};

/*Stores tag information*/
struct TIFF{
	unsigned short tag_id;
	unsigned short data_type;
	unsigned int num_items;
	unsigned int offset_items;
};

/*Stores information on the picture*/
struct JPEG_info{
	char manufacturer[100];
	char camera_model[100];
	int width;
	int height;
	int ISO;
	unsigned int exposure_speed[2];
	unsigned int fstop[2];
	unsigned int lens_focal_length[2];
	char date_taken[20];

};

int main(int argc, char *argv[]){

	FILE *image;

	//This will hold the spot in the file so we can traverse with ease
	unsigned int place_in_file; 

	/*If there is no file sumbitted, or too many arguments submitted. User
		will be asked to retry by rerunning the program*/
	if (argc != 2){ 
		printf("Please submit one JPEG file when running.\n");
		return 1;
	}

	image = fopen(argv[1], "rb"); // This is the file submitted

	struct Header jpeg_header; // Store the header info needed

	fread(&jpeg_header, sizeof(jpeg_header), 1, image); //Reads in header info

	/* Check to make sure file is the right type for the program.
		 If - Will be true if file is not JPEG, not APP1 or does not contain "Exif"
		 Else if - true if file is not little endian 
		*/
	if (jpeg_header.start_marker != 0xD8FF){
		printf("This type of file is not supported, please try a differnt JPEG file.\n");
		return 1;
	}
	else if (jpeg_header.app1_marker != 0xE1FF){
		printf("Only APP1 files accpeted, please try a differnt JPEG file.\n");
		return 1;
	}
	else if (strncmp("Exif", jpeg_header.exif_string, 4) != 0){
		printf("No exif data contained, please try a differnt JPEG file.\n");
		return 1;
	}
	else if (strncmp("II*", jpeg_header.endian_type, 4) != 0){
		printf("Only little endian files are supported, please try a differnt JPEG file.\n");
		return 1;
	}

	// File tests have passed

	struct TIFF tiff_tag; // Store individual tag info, will be reused for each tag
	struct JPEG_info picture_info; // Store specific file information

	/* For loop goes through each tag found and traverses through to get the data needed.
		int done is to specify whether we are done finding tags or not. 
		Seek to 22, where tags start */
	int done = 0; 
	fseek(image, 22, SEEK_SET);
	int i = 0;
	for (i = 0; i < jpeg_header.TIFF_count && !done; i++){
		//Tag IDs
		const unsigned short id_manufacturer = 0x010F, id_camera_model = 0x0110, id_exif_block = 0x8769,
			id_width = 0xA002, id_height = 0xA003, id_iso = 0x8827, id_exposure_speed = 0x829a,
			id_fstop = 0x829d, id_focal_length = 0x920A, id_date = 0x9003;
		
		fread(&tiff_tag, sizeof(tiff_tag),1,image); //read in tag
		
		// Check if it is one of the three tags we are looking for
		switch (tiff_tag.tag_id){
			case 0x010F: // Manufacturer tag
			{
				place_in_file = ftell(image); // save spot in traversal
				
				fseek(image, 12 + tiff_tag.offset_items, SEEK_SET); // seek to data 

				//This reads in each char one at a time into the char[]
				fread(&picture_info.manufacturer, sizeof(char), tiff_tag.num_items, image);
				
				fseek(image, place_in_file, SEEK_SET); // seek back to saved spot
				break; 
			}		
			case 0x0110: // Camera Model tag
			{
				place_in_file = ftell(image); // save spot in traversal
				 
				fseek(image, 12 + tiff_tag.offset_items, SEEK_SET); // seek to data 

				//This reads in each char one at a time into the char[]
				fread(&picture_info.camera_model, sizeof(char), tiff_tag.num_items, image);
			
				fseek(image, place_in_file, SEEK_SET); // seek back to saved spot
				break;
			}
			case 0x8769:
			{
				//Repeat process one last time 
				fseek(image, 12 + tiff_tag.offset_items, SEEK_SET);

				// new amount to loop through (number of tags in second exif block)
				unsigned short new_count;
				fread(&new_count, sizeof(new_count), 1, image);

				/* For loop goes through each tag found and traverses through to get the data needed.
					int done2 is to specify whether we are done finding tags or not. */
				int done2 = 0;
				int j = 0;
				for (j = 0; j < new_count && !done2; j++){

					fread(&tiff_tag, sizeof(tiff_tag), 1,image);
					/* Width, Height, ISO, datat is in offset_items. Others need to 
						seek to data and take it in, seek back. */
					switch (tiff_tag.tag_id){
						case 0xA002: // Width tag
						{
							picture_info.width = tiff_tag.offset_items;
							break;
						}
						case 0xA003: // Height tag
						{
							picture_info.height = tiff_tag.offset_items;
							break;
						}
						case 0x8827: // ISO tag
						{
							picture_info.ISO = tiff_tag.offset_items;
							break;
						}
						case 0x829a: // Exposure Speed tag
						{
							place_in_file = ftell(image); // save spot in traversal
							fseek(image, 12 + tiff_tag.offset_items, SEEK_SET); // seek to data 
							fread(&picture_info.exposure_speed, sizeof(picture_info.exposure_speed), 1, image);
							fseek(image, place_in_file, SEEK_SET);
							break;
						}
						case 0x829d: // F Stop tag
						{
							place_in_file = ftell(image); // save spot in traversal
							fseek(image, 12 + tiff_tag.offset_items, SEEK_SET); // seek to data 
							fread(&picture_info.fstop, sizeof(picture_info.fstop), 1, image);
							fseek(image, place_in_file, SEEK_SET);
							break;
						}
						case 0x920A: // Lens focal length tag
						{
							place_in_file = ftell(image); // save spot in traversal
							fseek(image, 12 + tiff_tag.offset_items, SEEK_SET); // seek to data 
							fread(&picture_info.lens_focal_length, sizeof(picture_info.lens_focal_length), 1, image);
							fseek(image, place_in_file, SEEK_SET); // seek back to saved spot
							break;
						}
						case 0x9003:
						{
							place_in_file = ftell(image); // save spot in traversal
							
							fseek(image, 12 + tiff_tag.offset_items, SEEK_SET); // seek to data 
							fread(&picture_info.date_taken, sizeof(picture_info.date_taken), 1, image);
							fseek(image, place_in_file, SEEK_SET); // seek back to saved spot

							done = 1;
							break;
						}

					} //end of inner switch

				} // end of inner for loop
				done = 1;
				break;
			}
		} // end of outer switch
	} // end of outer for loop

	//Print Picture Information
	printf("Manufacturer:    %s\n", picture_info.manufacturer);
	printf("Model:           %s\n", picture_info.camera_model);
	printf("Exposure Time:   %d/%d second\n", picture_info.exposure_speed[0], picture_info.exposure_speed[1]);
	double f_stop_decimal = (double)picture_info.fstop[0]/(double)picture_info.fstop[1];
	printf("F-stop:          %s%.1f\n", "f/", f_stop_decimal);
	printf("ISO:             ISO %d\n", picture_info.ISO);
	printf("Date Taken:      %s\n", picture_info.date_taken);
	double focal_decimal = (double)picture_info.lens_focal_length[0]/(double)picture_info.lens_focal_length[1];
	printf("Focal Length:    %.2f mm\n", focal_decimal);
	printf("Width:           %d pixels\n", picture_info.width);
	printf("Height:          %d pixels\n", picture_info.height);	

	//End of program
	return 0;
}