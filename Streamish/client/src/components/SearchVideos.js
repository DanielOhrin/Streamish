import { useState } from "react";
import { searchAllVideos } from "../modules/videoManager";

const SearchVideos = ({ setVideos }) => {
    const [query, setQuery] = useState(""),
        [desc, setDesc] = useState(false)

    const changeState = (event) => {
        if (event.target.name === "desc") {
            setDesc(event.target.value === "1") // 1 for true
        } else {
            setQuery(event.target.value)
        }
    }

    const executeSearch = () => {
        searchAllVideos(query, desc)
            .then(videos => setVideos(videos))
    }

    return (
        <>
            <h3 className="mt-4 d-flex mx-2">Search Videos</h3>
            <form className="mx-2 d-flex mb-4">
                <fieldset className="d-flex">
                    <div className="d-flex flex-column">
                        <label htmlFor="query">Query (% = Wildcard)</label>
                        <input type="text" name="query" value={query} onChange={changeState} />
                    </div>
                    <div className="d-flex flex-column mx-4">
                        <label htmlFor="desc">Sort</label>
                        <select defaultValue="1" onChange={changeState} name="desc">
                            <option value="0">Ascending</option>
                            <option value="1">Descending</option>
                        </select>
                    </div>
                </fieldset>
                <button type="button" id="search-btn" className="h-fit self-end h-full" onClick={() => executeSearch()}>Search</button>
            </form>
        </>
    )
}

export default SearchVideos