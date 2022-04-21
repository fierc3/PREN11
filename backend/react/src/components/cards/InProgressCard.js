import { CircularProgress, Typography } from "@mui/material";
import ArrowRightAltIcon from '@mui/icons-material/ArrowRightAlt';

const InProgressCard = (props) => {
    return (
        <>
            <CircularProgress sx={{ color: "purple", opacity: 0.4, marginTop: "5vh", marginLeft: "2vw", width:"10vw" }} />
            <Typography paddingLeft={"1vw"} gutterBottom variant="p" component="p">
                {props.text}
            </Typography>
        </>
    )
}

export default InProgressCard;