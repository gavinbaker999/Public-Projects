/*!
 * Joker
 * Copyright(c) 2021 End House Software
 * 
 * Version 1.0.0  Initial Version
 */

'use strict'

async function clickNextJoke() {
    const response = await $.post("/getjoke", function(data,status) {
        var jokeParts = data.split("#");

        $("#jokepunchline").hide(); 

        $("#joketext").text(jokeParts[0]); 
        $("#jokepunchline").text(jokeParts[1]);
    });
}

function clickShowPunchline() {
    $("#jokepunchline").show();
}
