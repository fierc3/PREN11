import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import { Typography } from '@mui/material';
import { Box } from '@mui/system';

const RunEndCard = (props) => {
    return (
        <Box sx={{marginTop: "75px", marginLeft: "50px"}}>
            <CheckCircleOutlineIcon sx={{ transform: "scale(2)", color: "green", opacity: 0.4 }} />
        </Box>
    )
}

export default RunEndCard;