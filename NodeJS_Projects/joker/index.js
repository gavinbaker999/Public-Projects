/*!
 * Joker
 * Copyright(c) 2021 End House Software
 * 
 * Version 1.0.0  Initial Version
 */

'use strict'

var express = require('express');
var exphbs  = require('express-handlebars');
var mysql   = require('mysql');
var path    = require('path');
var settings = require('./public/js/settings');

var app = express();

// View Engine
app.set('views', path.join(__dirname, 'views'));
app.engine('handlebars', exphbs({defaultLayout:'layout'}));
app.set('view engine', 'handlebars');

// Set Static Folder
app.use(express.static(path.join(__dirname, 'public')));

// Provide access to node_modules folder from the client-side
app.use('/scripts', express.static(`${__dirname}/node_modules/`));

app.get('/', function(req,res) {
    res.render('index');
});

app.post('/getjoke', function(req,res) {
	var connection = mysql.createConnection({
		host     : settings.dbHost,
		user     : settings.dbUser,
		password : settings.dbPassword,
		database : 'jokes'
	  });
	   
	  connection.connect(function(err) {
		  if (err) {
			console.error('error connecting: ' + err.stack);
			return;
		  }   
		  console.log('connected as id ' + connection.threadId);
	  }); 
	  
	  connection.query('USE jokes', function (error, results, fields) {
		  if (error) throw error;
	  }).catch((error) => {
		  console.error('MySQL Query Error Message: ${error}');
	  });

	  connection.query('SELECT jokeSetup,jokePunchline FROM jokedata ORDER BY RAND() LIMIT 1', function (error, results, fields) {
		if (error) throw error;
		res.send(results[0].jokeSetup + "#" + results[0].jokePunchline);
	  }).catch((error) => {
		console.error('MySQL Query Error Message: ${error}');
	  });
		 
	  connection.end();	  
});

// Set Port
app.set('port', (process.env.PORT || 3000));

app.listen(app.get('port'), function(){
	console.log('Server started on port '+app.get('port'));
});
