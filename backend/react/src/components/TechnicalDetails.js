import { Dialog, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import React from "react";


const TechnicalDetails = (props) => {
    const [scroll, setScroll] = React.useState('paper');
    return (<>
        <Dialog
            open={props.open}
            onClose={props.handleClose}
            scroll={scroll}
            aria-labelledby="scroll-dialog-title"
            aria-describedby="scroll-dialog-description"
        >
            <DialogTitle id="scroll-dialog-title">Events</DialogTitle>
            <DialogContent dividers={scroll === 'paper'}>
                <DialogContentText
                    id="scroll-dialog-description"
                    tabIndex={-1}
                >
                    {props.events
                        .map(
                            (x) => 'json: ' + JSON.stringify(x),
                        )
                        .join('\n')}
                </DialogContentText>
            </DialogContent>
        </Dialog>
    </>)
}


export default TechnicalDetails;