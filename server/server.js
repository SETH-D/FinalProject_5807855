var express = require('express');
var mySql = require ('mysql');
var app = express();
var util = require('util');

var connection = mySql.createConnection({
    host: 'ec2-52-221-186-20.ap-southeast-1.compute.amazonaws.com',
    user: 'devteam',
    password: 'password123',
    database: 'game'
});

connection.connect(function (err) {
    if (err) {
        console.log('Error Connecting', err.stack);
        return;
    }
    console.log('Connected as id', connection.threadId);

});

app.get('/users',function(req,res){
    queryAllUser(function(err,result){
        res.end(result);
    });
});

app.get('/user/add/user', function (req, res) {
    
    var username = req.query.username;
    var password = req.query.password;
    var playername = req.query.playername;

    var user = [[username,password,playername,0,""]];

    InsertUser(user,function(err,result){
        res.end(result);
    }); 
});

app.get('/user/updatescore', function (req, res) {
    
    var username = req.query.username;
    var score = req.query.score;

    UpdateScore(score, username, function(err,result){
        res.end(result);
    }); 
});

app.get('/user/updateprogress', function (req, res) {
    
    var save = req.query.save;
    var username = req.query.username;

    SaveProgress(save, username, function(err,result){
        res.end(result);
    }); 
});

app.get('/top10users', function (req, res) {
    queryTopTen(function(err,result){
        res.end(result);
    });
});

app.get('/login/:username/:password', function (req, res) {

    var username = req.params.username;
    var password = req.params.password;

    loginUser(function(err,result){
        res.end(result);
    }, username, password);
    
});

app.get('/user/getprogress/:username', function (req,res){

    var username = req.params.username;

    GetSave(function(err,result){
        res.end(result);
    },username);
})

var server = app.listen(8081, function () {
    console.log('Server: Running');
});

function loginUser(callback, username, password) {

    var json = '';
    var sql = util.format('SELECT username,playername,score,save FROM user WHERE username = "%s" AND password = "%s"', username, password);
    connection.query(sql,
        function (err, rows, fields) {
            if (err) throw err;

            json = JSON.stringify(rows);

            callback(null, json);
        });
}

function InsertUser(user,callback) {

    var sql = 'insert into user(username, password, playername, score, save) values ?';

    connection.query(sql,[user],
        function (err) {

            var result = '[{"success":"true"}]'

            if (err){
                result = '[{"success":"false"}]'
                throw err;

            }

            callback(null, result);
        });
}

function UpdateScore(score, username, callback){
    var sql = util.format('UPDATE user SET score = "%s" WHERE username = "%s"', score, username);

    connection.query(sql,
        function (err) {

            var result = '[{"success":"true"}]'

            if (err){
                result = '[{"success":"false"}]'
                throw err;

            }

            callback(null, result);
        });
}

function GetSave(callback, username){
    var sql = util.format("SELECT save FROM user WHERE username = '%s'", username);

    connection.query (sql,
        function (err, rows, fields) {
        if (err) throw err;

        json = JSON.stringify(rows);

        callback(null, json);
    });
}

function SaveProgress(save, username, callback){
    var sql = util.format("UPDATE user SET save = '%s' WHERE username = '%s'", save, username);

    connection.query(sql,
        function (err) {

            var result = '[{"success":"true"}]'

            if (err){
                result = '[{"success":"false"}]'
                throw err;

            }

            callback(null, result);
        });
}

function queryTopTen(callback){
    var json = '';
    connection.query("SELECT playername, score FROM user ORDER BY score DESC LIMIT 10;",
        function (err, rows, fields) {
            if (err) throw err;

            json = JSON.stringify(rows);

            callback(null, json);
        });
}

function queryAllUser (callback)
{
    var json = '';
    connection.query('SELECT * FROM user',
    function (err, rows, fields)
    {
        if (err) throw err;
        
        json = JSON.stringify(rows);

        callback(null,json);
    });
}
