I recommend running on a virtual linux machine for testing purposes.

Yahtzee and Yahtzee2 does not check for improper inputs.

When rerolling, format input would be :

> 1 2 4

if you want to reroll the 1st, 2nd, and 4th die

To stop rerolling, enter 0

Build Device Driver

Building driver on 32-bit linux kernel
> make ARCH = i386
> insmod device_driver_dev.ko
> cd /sys/class/misc/dice_driver
> cat dev
Take note of Major and Minor numbers
> cd /dev
> mknod dice c MAJOR MINOR
> rmmod dice_driver.ko
> rm /dev/dice


Running Yathzee with random driver
> gcc –m32 –o Yahtzee Yahtzee.c -static
> ./Yahtzee

Running yahtzee without random driver (used built in random generator)
> gcc –o Yahtzee2 Yahtzee2.c 
> ./Yahtzee2
