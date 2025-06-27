# My Approach
My program asks the user for the location of the log file, since it changes depending on what system is running it.
It displays the event type counts in the log and then asks the user what event type they want the most common phrases for.

I created a log parser class that parses the log stored at the file path provided by the user.
It turns each log line into a log entry struct that stores the extracted information.

I then created a static class called "LogAnalyzer" that is used to print information using the parsed log data.
I made this a seperate class because parsing data and displaying data are two different responsibilites,
and thus should be handled separately.

The log analyzer uses a dictionary to keep track of the event type counts.
It does the same exact thing to keep track of the most common message phrases too.

# Assumptions
With this program, I've assumed that the path to the log file is known and accessible to the user.
I've also assumed that the log format will not change, and that log messages can never contain new line characters.

# Known Limitations
- My program will crash if the user provides improper input.
- My program will break if the log message contains a new line character.
- My program cannot find the log file on its own.
- The memory useage of my program will grow proportionally to log file size.
This can be an issue with impressively large log files.
