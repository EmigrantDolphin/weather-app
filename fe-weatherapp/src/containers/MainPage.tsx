import React, { useEffect, useState } from "react";
import { CityPrediction, Weather, getTodayWeather, getCityName, getPredictions } from "../api/apiEndpoints";
import { SearchBar } from "../components/SearchBar";
import { WeatherDisplay } from "../components/WeatherDisplay";

type Coords = {
    latitude: number,
    longitude: number
}
type Position = {
    coords: Coords
}

export const MainPage: React.FC = () => {
    const [todayWeather, setTodayWeather] = useState<Weather | null>(null);
    const [todayCityName, setTodayCityName] = useState<string>("");

    useEffect(() => {
        navigator.geolocation.getCurrentPosition(loadWeatherByDeviceLocation)
    }, [])

    const loadWeatherByDeviceLocation = (position: Position) => {
        // I can explain this long spaghetto
        getCityName(position.coords.latitude, position.coords.longitude)
            .then(cityName => {
                getPredictions(cityName)
                    .then(predictions => {
                        if (predictions.length > 0) {
                            getTodayWeather(predictions[0].placeId)
                                .then(weather => {
                                    setTodayWeather(weather);
                                    setTodayCityName(predictions[0].cityName);
                                })
                        }
                    })
            })    
    }

    const onCitySelected = (prediction: CityPrediction) => {
        setTodayCityName(prediction.cityName);
        getTodayWeather(prediction.placeId)
            .then(weather => {
                setTodayWeather(weather);
            })
    }

    return (
    <div>
        <SearchBar onCitySelected={onCitySelected} />
        <h3>Weather today</h3>
        {todayWeather && (
            <WeatherDisplay weather={todayWeather} cityName={todayCityName}/>
        )}
    </div>
    );
}