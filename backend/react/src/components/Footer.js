import { Box, Typography } from "@mui/material";


const Footer = (props) => {
    return (<>
        {/* Footer */}
        <Box sx={{position: "fixed", bottom: 0, height: "10vh", width: "100vw", borderTop: 1, borderColor: '#d2d2d9', backgroundColor:"white"  }} component="footer">
            <Typography sx={{pt:"10px"}} fontWeight="light" variant="h4" align="center" gutterBottom>
                <span className="largeTitle">ROBOT IS CURRENTLY</span> <Typography variant="span" color="green">
                    ONLINE
                </Typography>
            </Typography>
        </Box>
        {/* End footer */}
    </>)
}


export default Footer;