
// DB
const sqlite3 = require('sqlite3').verbose();

let db = undefined;
let io = undefined;

const init = (socketIO) => {

    if (db === undefined) {
        console.log("init db connection");
        db = new sqlite3.Database('./pren11.db');
    }

    if(socketIO !== undefined){
        io = socketIO;
    }
}

const getDb = () => {
    if (!db) {
        init();
    }
    return db;
}

const getCurrentRun = (callBack) => {
    getDb().all(`SELECT * FROM event ORDER BY event_id DESC LIMIT 1;`, [], (errors, rows) => {
        console.log("rows", rows);
        if (rows[0]) {
            callBack(rows[0].run)
            return rows[0].run;
        } else {
            return undefined;
        }
    });
}


const addEvent = (event, callback) => {
    console.log("Adding Event:", event)
    const save_date = new Date();
    var stmt = getDb().prepare(`
    INSERT INTO event ( run, event_type, event_value, datetime) 
    VALUES (?, ?, ?,?)`);
    stmt.run([event.run, event.event_type, event.event_value, save_date.toISOString()], (error) => {
        if (error) {
            console.warn("ADDING EVENT FAILED!", error)
        } else {
            callback(event);
        }
    })

}

const sendRobotOutput = (event) => {
    io.emit("RobotOutput",JSON.stringify(event));
}

const markBeginRun = (latestRun) => {
    let runId = 1;
    if (latestRun !== undefined) {
        runId = latestRun + 1;
    }
    console.log("Setting new RunId: ", runId);
    const event = { run: runId, event_type: 1, event_value: null }
    addEvent(event,sendRobotOutput)

}

const markEndRun = (latestRun) => {
    if (latestRun === undefined) {
        console.log("Trying to end run which doesn't exit")
        return;
    }
    const event = { run: latestRun, event_type: 3, event_value: null };
    addEvent(event,sendRobotOutput)
}

const insertPlant = async (plantData) => {
    console.log("Adding Plant", plantData)
    getCurrentRun((latestRun) => {
        if (latestRun === undefined) return;
        const event = { run: latestRun, event_type: 2, event_value: plantData };
        addEvent(event,sendRobotOutput);
    })
}

const beginRun = async () => {
    console.log("Beginning Run---------")
    getCurrentRun(markBeginRun);
}

const endRun = async () => {
    console.log("-----------Ending")
    getCurrentRun(markEndRun);
}

const getEventsFromCurrentRun = (callBack) => {
    getCurrentRun((run) =>{
        if(run === undefined){
            callBack(undefined);
        }else{
            getDb().all(`SELECT * FROM event WHERE run = ?;`, [run], (errors, rows) => {
                console.log("rows", rows);
                callBack(rows);
            });
        }
    } );
}


module.exports = { beginRun, endRun, addEvent, init, insertPlant, getEventsFromCurrentRun };