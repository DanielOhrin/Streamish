import React, { useEffect, useState } from "react"
import { getAllVideos } from "../modules/videoManager"
import SearchVideos from "./SearchVideos"
import Video from "./Video"

const VideoList = () => {
    const [videos, setVideos] = useState([])

    const getVideos = () => {
        getAllVideos().then(videos => setVideos(videos))
    }

    useEffect(() => {
        getVideos()
    }, [])

    return (
        <>
            <SearchVideos setVideos={setVideos} />
            <div>
                {videos?.map(v => <Video video={v} key={`video--${v.id}`}></Video>)}
            </div>
        </>
    )
}

export default VideoList