import React from "react"
import { Card, CardBody, CardFooter } from "reactstrap"
import Comment from "./Comment"

const Video = ({ video }) => {
    return (
        <Card>
            <p className="text-left px-2">Posted by: {video.userProfile.name}</p>
            <CardBody>
                <iframe className="video"
                    src={video.url}
                    title="YouTube video player"
                    frameBorder="0"
                    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                    allowFullScreen />

                <p>
                    <strong>{video.title}</strong>
                </p>
                <p>{video.description}</p>
            </CardBody>
            {(
                video.comments?.length
                    ? <CardFooter>
                        <h3>Comments</h3>
                        <div>
                            {video.comments.map(c => <Comment comment={c} key={`comment--${video.id}--${c.id}`} />)}
                        </div>
                    </CardFooter>
                    : <h3>No Comments</h3>
            )}
        </Card>
    )
}

export default Video