/*
 * "Hello, world!" minimal kernel module - /dev version
 *
 * Valerie Henson <val@nmt.edu>
 ** edited by Nicholas Smider to make Dice Driver
 */

#include <linux/fs.h>
#include <linux/init.h>
#include <linux/miscdevice.h>
#include <linux/module.h>
#include <linux/random.h>

#include <asm/uaccess.h>

/*
 * dice_driver_read is the function called when a process calls read() on
 * /dev/dice_driver.  It writes a random number 1-6 to the buffer passed in the
 * read() call.
 */

unsigned char get_random_byte(int max) { 
	unsigned char c;
	get_random_bytes(&c, 1); 
	return c%max;
}

static ssize_t dice_driver_read(struct file * file, char * buf, 
			  size_t count, loff_t *ppos)
{
	int max = 6;
	int roll = (int)get_random_byte(max);
	roll++;

	if (roll < 0){
		roll *= -1;
	}
	/*
	 * We only support reading the whole string at once.
	 */
	if (count < 1)
		return -EINVAL;
	/*
	 * If file position is non-zero, then assume the string has
	 * been read and indicate there is no more data to be read.
	 */
	//*ppos++;
	//if (*ppos != 0)
	//	return 0;
	/*
	 * Besides copying the string to the user provided buffer,
	 * this function also checks that the user has permission to
	 * write to the buffer, that it is mapped, etc.
	 */
	if (copy_to_user(buf, &roll, 1))
		return -EINVAL;
	/*
	 * Tell the user how much data we wrote.
	 */
	*ppos = 1;

	return 1;
}

/*
 * The only file operation we care about is read.
 */

static const struct file_operations dice_driver_fops = {
	.owner		= THIS_MODULE,
	.read		= dice_driver_read,
};

static struct miscdevice dice_driver_dev = {
	/*
	 * We don't care what minor number we end up with, so tell the
	 * kernel to just pick one.
	 */
	MISC_DYNAMIC_MINOR,
	/*
	 * Name ourselves /dev/dice_driver.
	 */
	"dice_driver",
	/*
	 * What functions to call when a program performs file
	 * operations on the device.
	 */
	&dice_driver_fops
};

static int __init
dice_driver_init(void)
{
	int ret;

	/*
	 * Create the "dice_driver" device in the /sys/class/misc directory.
	 * Udev will automatically create the /dev/dice_driver device using
	 * the default rules.
	 */
	ret = misc_register(&dice_driver_dev);
	if (ret)
		printk(KERN_ERR
		       "Unable to register \"Dice Driver\" misc device\n");

	return ret;
}

module_init(dice_driver_init);

static void __exit
dice_driver_exit(void)
{
	misc_deregister(&dice_driver_dev);
}

module_exit(dice_driver_exit);

MODULE_LICENSE("GPL");
MODULE_AUTHOR("Valerie Henson <val@nmt.edu>"); //Edited by Nicholas Smider
MODULE_DESCRIPTION("\Dice Driver\" minimal module");
MODULE_VERSION("dev");
