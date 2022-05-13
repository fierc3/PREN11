import { Box, Grid, Typography } from "@mui/material";
import LinearProgress from '@mui/material/LinearProgress';


const Footer = (props) => {
    return (<>
        {/* Footer */}
        <Box sx={{ position: "fixed", bottom: 0, height: "15vh", width: "100vw", borderTop: 1, borderColor: '#d2d2d9', backgroundColor: "white" }} component="footer">
            <Typography sx={{ pt: "10px" }} fontWeight="light" variant="h4" align="center" gutterBottom>
                <span className="largeTitle">ESTIMATE</span> <Typography variant="span" color="green">
                    PROGRESS: {props.progress}%
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