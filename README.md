# LogsAnalyzer
Logs Analyzer provides the ability to **filter** and **analyze** logs. The analyzer can be run in one of two ways; the first one as an extension to your application logger 
and in the same app domain as your application, and the other one in a separate process isolated from your application.

The library becomes more helpful in cases or scenarios where the logs are huge and crowded which make it's hard to read them.The library supports multiple ways for filtering
and analyzing the logs in addition to support reporting the output of the analyzer results in different forms.

### Logs Filters:
Sometimes it's helpful to report only part of the logs that is related to a specific feature or service. Logs Analyzer gives the user the ability to identify a specific log
by configuring the filters that match this log. 
The filters can be on log message, level, tags, specific parameters.

### Logs Analyzer Rules:
The following are the supported rules:
- Absence Detection Rule: triggers an output if a specific log is not received within a configured interval in seconds.
- Aggregate Function Rule: triggers an output when a specific log is received along with a value calculated from a specific parameter passed with the log and using the configured
  aggregate funciton.
- Anti Sequence Rule:
- Duplicate Detection Rule: triggers an output when a specific log is received twice.
- Filters Rule:
- Frequency Rule:
- Sequence Detection Rule:
- Time Difference Rule: triggers an output after each time a specific log is received along with the interval in seconds between the new one and the previous one.

### Logs Outputs:
- Text, CSV, and HTML files.
- Windows Popup Messages.
- Emails.
