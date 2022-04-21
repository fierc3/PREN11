import { AppBar, Toolbar, Typography } from "@mui/material";

const NavigationBar = (props) => {
    return (<AppBar position="fixed">
        <Toolbar id="toolbar">
            <Typography
                id='smallTitle'
                component="h2"
                variant="h5"
                color="inherit"
                align="left"
                noWrap
                sx={{ flex: 1 }}
            >🤖</Typography>
            <span className="largeTitle">
            <Typography
                component="h2"
                variant="h5"
                color="inherit"
                align="left"
                noWrap
                sx={{ flex: 1 }}
                fontWeight="light"
            >PREN by Team 11 🤖</Typography>
            </span>
        </Toolbar>
    </AppBar>);
}

export default NavigationBar;