var createError = require('http-errors');
var express = require('express');
var path = require('path');
var cookieParser = require('cookie-parser');
var logger = require('morgan');
var cors = require('cors')

var indexRouter = require('./routes/index');
var usersRouter = require('./routes/users');
const { getEventsFromCurrentRun } = require('./db');

var app = express();
app.use(cors())

//init react
// serve the react app files
console.log("initing react files")
app.use(express.static(`${__dirname}/react/build`));
app.get('/', (req, res) => res.sendFile(path.resolve('react', 'build', 'index.html')));
app.get('/admin', (req, res) => res.sendFile(path.resolve('react', 'build', 'index.html')));


app.get('/api/currentRun', (req,res) => {
  console.log("Current Run Data Requested");
  getEventsFromCurrentRun((events) => {
    res.status(200).json(events)
  })
})


/*
// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'pug');
*/
app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

/*
app.use('/', indexRouter);
app.use('/users', usersRouter);
*/


// catch 404 and forward to error handler + cors
app.use(function(req, res, next) {
  res.header('Access-Control-Allow-Origin', "*");
  res.header('Access-Control-Allow-Methods', 'GET,PUT,POST,DELETE');
  res.header('Access-Control-Allow-Headers', 'Content-Type');

  next(createError(404));
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

module.exports = app;
