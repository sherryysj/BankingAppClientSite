# RMIT-WDT-Assignment2 Group # 29
**Group member details**\
Shujie Yang - s3704732\
Siyuan Cao - s3502747

**About this project**\
This project is developed based on the sample code of RMIT WDT-2021 flexible semester, Tourtial Week6 - "McbaExampleWithLogin". We have a good understanding of the code structures and the techniques used in this template.

**About branching**\
We have created and used different branches for each part of this Assignment, e.g. 'Pass-part', 'HD-part'.
As recommended, the Admin site and API are developed as a separated project which is located in the 'Distinction-part' branch of this repository, where as the Client site is in the master branch.

**The use of Records**\
In this project Records are used to replace regular Model classes for two main reasons. \
First, Record classes provide immutability to Models. The variables can only be modified by using keyword "with" after initialization. This prevents unintentional changes or changes of values by mistake throughout the program.
Secondly, Records have better performance than normal classes. They are passed as references not copies thus memory is saved. Also there is a significant performance boost as well.

**Database modifications**\
There are seven migrations created throughout the development(from migration1.0 to 1.6), the major modifications made are:\
'Active' columns are added to the 'Account' and 'BillPays' tables so users and bills can be blocked when needed.\
'Email' and 'CheckDate' are added to Customers to support the implimentations of assignment part-k.\
'Payee' table is also created for creating billpays.

**Client site Login details**\
We have modified the given login details so the Login IDs are easier to remember, thus helps debugging and testing processes(the passwords and hashed passwords stay the same).
The modified login IDs are:\
12345678 for Customer 2100,\
23456789 for Customer 2200,\
34567890 for Customer 2300.

**Admin site Login details**\
As stated in the Assignment specification, the admin login is hardcoded in the 'Bank Web API' project. To login to the Admin site please use:\
Username: 123\
Password: abc123

**Payees**\
Two payees are seeded into the database, which are:\
1	RMIT.EDU		124 La Trobe St	Melbourne	VIC	3000	+61 488888888\
2	Rent Payment System	123 Bob St	Melbourne	VIC	3100	+61 400000000

**Email Function Credential**\
We use SMTP service to send email report for customers and register a gmail account for server sending emails.\
The gmail account credential for reference:\
Email: rmitwdta2@gmail.com\
Password: s3704732

**Plugin For Page Control**\
We use X.PagedList to do page control on View/Customer/Statement.cshtml for only showing 4 transaction at on one page.

**Unaccomplished function**\
The requirment-j of this assignment was not fully achieved due to some issues and time. The new migrations could not be generated after introduce Identity API, and there are errors occured during DB update.
