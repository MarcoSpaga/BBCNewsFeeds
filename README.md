# BBCNewsFeeds
A console application that reads BBC News feeds and puts them into files. 


Design:

Parent and child classes were created to handle the information coming from feeds.

A helper class is created to help with processing the feeds. This enables the main class to be small and easily readible.

There was a serialisation issues in the CreateJsonFile method where it appeared to write out two parent elements.
I think the cause of this is down to a the serialisation method reading both the method's local copy of the parent object (News) and the heap. As such, I have had to move the functionality of this method into the main class which makes things less tidy and succinct. 

If you know the answer to this serialisation problem or have any suggestions please drop me an email. gianmarcospagnoletti@gmail.com

The program runs on the hour every hour and uses the static helper class to do the dirty work of writing to files, searching for duplicate news feeds, and reading the feeds. All files created are stored in a folder called 'feeds' that is stored on the desktop.

After running for 24 hours the application will remove all files stored in the 'feeds' folder. 
