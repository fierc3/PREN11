import { Box, Grid, Typography } from "@mui/material";
import LinearProgress from '@mui/material/LinearProgress';


const Footer = (props) => {
    return (<>
        {/* Footer */}
        <Box sx={{ position: "fixed", bottom: 0, height: "20vh", maxHeight:'120px', width: "100vw", borderTop: 1, borderColor: '#d2d2d9', backgroundColor: "white" }} component="footer">
            <Typography sx={{ pt: "10px" }} fontWeight="light" variant="h4" align="center" gutterBottom>
                <span className="largeTitle">ESTIMATE</span> <Typography variant="span" color="green">
                    PROGRESS: {props.progress}% ({props.runDuration})
                </Typography>
            </Typography>
            <Grid container justifyContent="center">
                <Box sx={{ width: '70%', alignSelf: 'center' }}>
                    <LinearProgress variant="determinate" value={props.progress} />
                </Box>
            </Grid>

        </Box>
        {/* End footer */}
    </>)
}


export default Footer;