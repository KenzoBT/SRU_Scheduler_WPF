# SRU_Scheduler_WPF
SRU Term Scheduler (C# + WPF/XAML)

You can download the standalone executable (x64 Win) for this application [here](https://www.dropbox.com/s/unarn5k29wb6gqq/SRU-TermScheduler.zip?dl=0).

This application allows department chairs to schedule classes for the semester.
The input file required is a formatted .xlsx or .csv file like the one [here](https://www.dropbox.com/s/56u9hzpvxjrq1qi/CPSC%20-%20Fall%202021%20-%20Example.xlsx?dl=0 "Excel file").

![Application](http://myxos.live/app1.png)

The application lets the user drag-and-drop courses into the timeslot/class pairing that they wish. If there are any time conflicts or professor conflicts, the app will prevent the action and show a message explaining the conflict.

Data related to professors is also managed to prevent over or under scheduling them for classes.

![Professors](http://myxos.live/prof.png)

Professor preferences can be included in the Adv. Options tab. If a professor is scheduled a class that he/she is not comfortable teaching, a message will appear to let the user know of this problem. An example preference file can be downloaded [here](https://www.dropbox.com/s/tras13i9oi3uvpj/Teaching%20Prefs%20Example.xlsx?dl=0).

![Advanced Options](http://myxos.live/additional.png)
