import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { addVideo } from "../modules/videoManager"

const VideoForm = () => {
    const [newVideo, setNewVideo] = useState({ title: "", description: "", url: "" }),
        navigate = useNavigate()

    // State change function that works for any value in the newVideo object
    const changeState = (e) => {
        const copy = { ...newVideo }
        copy[e.target.name] = e.target.value

        setNewVideo(copy)
    }

    const addNewVideo = () => {
        addVideo(newVideo)
            .then(res => {
                if (res.ok) { // If the http request returns a good status code, we redirect them back to the list of videos
                    navigate("/")
                }
            })
    }

    return (
        <>
            <hr />
            <div className="d-flex flex-column align-items-start mx-2">
                <h2>New Video</h2>
                <form>
                    <fieldset>
                        <div>
                            <label htmlFor="title">Title</label>
                            <input className="my-2 w-full" name="title" onChange={changeState} value={newVideo.title} />
                        </div>
                        <div>
                            <label htmlFor="url">YouTube Url</label>
                            <input className="my-2 w-full" name="url" onChange={changeState} value={newVideo.url} />
                        </div>
                        <div>
                            <label htmlFor="description">Description (Optional)</label>
                            <input className="my-2 w-full" name="description" onChange={changeState} value={newVideo.description} />
                        </div>
                    </fieldset>
                    <button type="button" onClick={() => addNewVideo()}>Add Video</button>
                </form>
            </div>
        </>
    )
}

export default VideoForm