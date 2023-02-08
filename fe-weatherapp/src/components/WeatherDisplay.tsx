import React from "react"
import { Weather } from "../api/apiEndpoints"
import "./WeatherDisplay.css";

type Props = {
    weather: Weather | null;
    cityName: string;
}

export const WeatherDisplay: React.FC<Props> = (props: Props) => {
    if (!props.weather) {
        return (<></>);
    }

    const temp: number = props.weather.temperatureKelvin - 273.15;
    const tempFeelsLike: number = props.weather.temperatureKelvinFeelsLike - 273.15;

    return (
        <>
            <h4>{props.cityName}</h4>
            <table style={{width: '300px'}}>
                <tbody>
                    <tr>
                        <td rowSpan={2}>
                            {props.weather.mainName}
                        </td>
                        <td className="second-column">
                            Temperature: {temp.toFixed(2)}°
                        </td>
                    </tr>
                    <tr>
                        <td className="second-column">
                            Feels like: {temp.toFixed(2)}°
                        </td>
                    </tr>
                    <tr>
                        <td rowSpan={2}>
                            {props.weather.description}
                        </td>
                        <td className="second-column">
                            humidity: {props.weather.humidity}%
                        </td>
                    </tr>
                    <tr>
                        <td className="second-column">
                            Wind speed: {props.weather.windSpeed}m/s
                        </td>
                    </tr>
                </tbody>
            </table>
        </>
    );
}