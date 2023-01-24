import { CardText } from "reactstrap"

const Comment = ({comment}) => {
    return (
        <div>
            <h5>{comment.userProfile.name}</h5>
            <CardText>{comment.message}</CardText>
        </div>
    )
}

export default Comment