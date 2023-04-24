# Joker

Fathom full stack developer task.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

To run locally you need the [MySQL server](https://dev.mysql.com/downloads/) to be installed. 

### Database Structure

To import joke data into database and create the jokedata table, change to the public\js directory and execute

    $mysql --local_infile=1 -u root -p < dbcreate.sql

Note: This assumes you are loging on to the MySQL database server as user 'root', you will be prompted to enter a password. If you recieve an error importing the joke data, you may have to check if the local_infile global variable is disabled. To enable local_infile,

    mysql> SET GLOBAL local_infile=1;
    Query OK, 0 rows affected (0.00 sec)

Update the file public\js\settings.js with the database host and logon details .The database table name can not be changed.

## Build Procedures

The following describes the build procedures.

### Command Line Build:

To install all the required NodeJS modules, you only need to do this once or to update your installation.

	$npm install

To run the application,

	$npm start
    
    Use your web browser to view http://localhost:3000

## Built With

* [Visual Studio Code](https://code.visualstudio.com/
) - IDE enviroment.

## Versioning

We use [SemVer](http://semver.org/) for versioning.  

## Authors

* **Gavin Baker** - *Initial work* - [End House Software](endhousesoftware.byethost11.com).
