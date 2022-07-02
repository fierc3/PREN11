import '../App.css';
import { config } from '../Constants';
import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import Footer from './Footer';
import NavigationBar from './NavigationBar';
import { Box, width } from '@mui/system';
import { Alert, Grid, Snackbar, Typography } from '@mui/material';
import RunEndCard from './cards/RunEndCard';
import InProgressCard from './cards/InProgressCard';
import PlantCard from './cards/PlantCard';
import { getMainSocket } from './MainSocketSingleton';
import socketIOClient from "socket.io-client";
import useCheckMobileScreen from './Utils';
import TechnicalDetails from './TechnicalDetails';


var ENDPOINT = config.url.API_URL;

const Guest = () => {
    const theme = createTheme({
        typography: {
            fontWeightLight: "300"
        },
        palette: {
            primary: {
                light: '#757ce8',
                main: '#6200EE',
                dark: '#002884',
                contrastText: '#fff',
            },
            secondary: {
                light: '#ff7961',
                main: '#f44336',
                dark: '#ba000d',
                contrastText: '#000',
            },
        },
    });

    const [data, setData] = React.useState([])
    const [run, setRun] = React.useState("is starting")
    const [startDate, setStartDate] = React.useState("")
    const [toast, setToast] = React.useState(false)
    const [toastText, setToastText] = React.useState(false)
    const [progress, setProgress] =  React.useState(0);
    const [openDetails, setOpenDetails] = React.useState(false);
    const [elapsedTime, setElapsedTime] = React.useState(new Date())
    const [durationPrint, setDurationPrint] = React.useState("")

    React.useEffect(() => {
        if(!data) return
        const end = data?.find(e => e.event_type === 3)
        const start = data[0]
        if(!start) return;
        console.log("start",start)
        console.log("end",end)
        let dateToPrint = elapsedTime;
        if(end){
            dateToPrint = new Date(new Date(end.datetime).getTime()-new Date(start.datetime).getTime());
        }
        setDurationPrint(dateToPrint.getMinutes() + "M:" + dateToPrint.getSeconds()+"S")
    },[elapsedTime])

    let timerInterval = undefined;

    const updateMetaData = (event) => {
        setRun(event.run);
        let date = new Date(event.datetime)
        setStartDate("Run started at " + date.toLocaleString());
        clearInterval(timerInterval)
        timerInterval = setInterval(() => {
            console.log(data.filter(e => e.event_type === 3).length)
            const duration = new Date(new Date().getTime() - date.getTime());
            console.log("duration", duration)
            setElapsedTime(duration)
        }, 1000)
    }

    const updateProgress=(events)=>{
        if(events.find(x => x.event_type === 3)){
            setProgress(100)
            return;
        }
        setProgress(events.length  / (events.length + 3)* 100);
    }

    const connect = () => {
        let socket = socketIOClient(ENDPOINT, { transports: ['websocket'], secure: true });
        socket.on("RobotOutput", roboMessage => {
            
            let json = JSON.parse(roboMessage);
            setData(arr => [...arr, json]);
            if (data.length < 1 || json.run !== data[0].run) {
                //new run, update whole app
                updateData();
            }
            if (json.event_type === 1) {
                setToastText("New Run Started!")
                setProgress(1);
            } else if (json.event_type === 2) {
                const plant = JSON.parse(json.event_value);
                setToastText(`Found ${plant.plantName}!`)
                updateProgress(data);
            } else if (json.event_type === 3) {
                setToastText("Run has finished")
                setProgress(100)
            }
            setToast(true)

        });
    }


    const updateData = async () => {
        try {
            const response = await fetch(ENDPOINT + "/api/currentRun");
            const json = await response.json();
            setData(json);
            console.log(json)
            updateProgress(json)
            if (json.length > 0) {
                updateMetaData(json[0])
            }
        }catch (ex){
            console.log("nodata")
        }


    }

    React.useEffect(() => {
        updateData();
        connect();
    }, [])

    let firstPlantName = "";

    /*
    hint for mobile grid
        align="center"
    justify="center"
    direction="column"
    */

    return (
        <>
            <ThemeProvider theme={theme} >
                <CssBaseline />
                <NavigationBar />
                <main style={{display:'inline'}}>
                    <Box sx={{ pt: "10vh", backgroundColor: "", height: "90vh" }}>
                        <Typography gutterBottom variant="h5" component="span" position={"absolute"} paddingLeft={"30px"} paddingTop={"50px"}>
                            Run {run}
                        </Typography>
                        <Typography gutterBottom variant="caption" component="span" position={"absolute"} paddingLeft={"30px"} paddingTop={"75px"}>
                            {startDate}
                        </Typography>
                        <Typography gutterBottom variant="caption" component="span" position={"absolute"} paddingLeft={"30px"} paddingTop={"100px"}>
                            This is the offical website for PREN by group 11. Click <a style={{textDecoration:'underline', cursor:'pointer'}} onClick={() => setOpenDetails(true)}>here</a> for detailed technical view!
                        </Typography>
                        <Box sx={{marginTop: "27vh", marginLeft: "50px", paddingBottom:"100px"}}>
                            <Grid container spacing={6} alignContent="center"direction={useCheckMobileScreen() ? "column": undefined} style={{width:"100%",  paddingBottom:'100px'}} >
                                
                                {data.filter(event => event.event_type !== 1).map((event, index) => {
                                    const plant = JSON.parse(event.event_value);

                                    console.log("plant", plant)
                                    if (index === 0 && plant) {
                                        //first plant found, so needs to be the one at the start
                                        firstPlantName = plant.plantName;
                                        if(firstPlantName)
                                        return (<Grid item key={event.event_id}>
                                            <PlantCard name={plant.plantName} image={plant.image} text={"Found at start"} />
                                        </Grid>)
                                    }
                                    if (index > 0 && event.event_type === 2) {
                                        return (
                                            <Grid item key={event.event_id}>
                                                <PlantCard name={plant.plantName}  image={plant.image} text={"Found at Position " + index} found={firstPlantName === plant.plantName} />
                                            </Grid>
                                        )
                                    } else if (event.event_type === 3) {
                                        return (
                                            <Grid item key={event.event_id}>
                                                <RunEndCard />
                                            </Grid>
                                        )
                                    }
                                })}
                                {data.filter(e => e.event_type === 3).length === 0 &&
                                    <Grid key={69669} item><InProgressCard text={"Searching..."} />
                                    </Grid>}
                            </Grid>
                        </Box>
                    </Box>
                    
                </main>
                <Snackbar
                    anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
                    open={toast}
                    autoHideDuration={3500}
                    onClose={() => setToast(false)}
                    sx={{ zIndex: 9999 }}
                >
                    <Alert severity="success">{toastText}</Alert>
                </Snackbar>
                <Footer  progress={parseFloat(progress.toFixed(2))} runDuration={durationPrint} />
                <TechnicalDetails events={data} open={openDetails} handleClose={() => setOpenDetails(false)} ></TechnicalDetails>
            </ThemeProvider>
        </>
    );
}

export default Guest;
