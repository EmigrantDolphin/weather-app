import React, { useRef, useState } from "react";
import { CityPrediction, getPredictions } from "../api/apiEndpoints";
import "./SearchBar.css";

type Props = {
    onCitySelected: (prediction: CityPrediction) => void
};

export const SearchBar: React.FC<Props> = (props: Props) => {

    const [predictions, setPredictions] = useState<Array<CityPrediction>>([]);
    const [isSearching, setIsSearching] = useState<boolean>(false);
    const [isOnFetchCooldown, setIsOnFetchCooldown] = useState<boolean>(false);
    const [searchInput, setSearchInput] = useState<string>("");

    const searchInputRef = useRef(searchInput);
    searchInputRef.current = searchInput;

    const onOptionSelect = (prediction: CityPrediction) => {
        setIsSearching(false);

        setSearchInput(prediction.cityName);

        props.onCitySelected(prediction);
    }

    const onSearchInput = (e: React.FormEvent<HTMLInputElement>) => {
        setSearchInput(e.currentTarget.value);
        if (e.currentTarget.value.length <= 0) {
            setPredictions([]);
        }

        if (!isOnFetchCooldown) {
            setIsOnFetchCooldown(true);

            setTimeout(() => {
                setIsOnFetchCooldown(false);

                if (searchInputRef.current.length > 0) {
                    getPredictions(searchInputRef.current)
                        .then(predictions => setPredictions(predictions));
                }
            }, 1500)
        }
    }

    const renderOptions = () => {
        if (isSearching) {
            return (
                <div className="options">
                    {predictions.map((prediction, i, row) => {
                        return (
                            <p
                                className={`search-option ${i + 1 === row.length && "last-option"}`}
                                onClick={() => onOptionSelect(prediction)}
                                key={i}>{prediction.cityName}
                            </p>
                        )
                    })}
                </div>
            );
        }

        return <div></div>
    }

    return (
        <div className="search-bar-wrap">
            <input
                type="text"
                className={`search-bar ${isSearching && predictions.length > 0 && 'options-present'}`}
                onClick={() => setIsSearching(true)}
                onInput={onSearchInput}
                value={searchInput}
                placeholder="Enter city name"
            />
            {renderOptions()}
        </div>
    );
}