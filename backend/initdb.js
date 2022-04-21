const sqlite3 = require('sqlite3').verbose();

console.log('Started init of sqllite database')
// open a database connection
let db = new sqlite3.Database('./pren11.db');


// creating event table
let createEventsTable=`
CREATE TABLE event (
    event_id INTEGER PRIMARY KEY,
    run INTEGER NOT NULL,
    datetime TEXT NOT NULL,
    event_type INTEGER NOT NULL,
    event_value TEXT
);
`;

db.exec(createEventsTable, (err) => {
    if(err){
        console.warn("Error while creating event table", err)
    }else{
        console.log("Successfully created db");
    }
})
