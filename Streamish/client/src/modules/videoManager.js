import { getToken } from "./authManager"

const baseUrl = '/api/video'

export const getAllVideos = () => {
    return getToken().then(token => {
        return fetch(`${baseUrl}/getwithcomments`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        }).then(res => res.json())
    })
}

export const addVideo = (video) => {
    return getToken().then(token => {
        fetch(baseUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(video)
        })
    })
}

export const searchAllVideos = (queryStr, descBool) => {
    return getToken().then(token => {
        fetch(`${baseUrl}/search?sortDesc=${descBool}&q=${(queryStr ?? "%%")}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        })
            .then(res => res.json())
    })
}