const baseUrl: string = `${import.meta.env.VITE_WEATHERAPP_API_URL}api/`;

export const getCityName = async (latitude: number, longitude: number): Promise<string> => {
    const url: string = `${baseUrl}location/city-name?longitude=${longitude}&latitude=${latitude}`;

    const response: Response = await get(url);

    if (response.status === 200) {
        const cityName: string = await response.text();

        return cityName;
    }

    return Promise.reject(await response.text())
}

export type CityPrediction = {
    placeId: string;
    cityName: string;
}

export const getPredictions = async (incompleteCityName: string): Promise<Array<CityPrediction>> => {
    const url: string = `${baseUrl}location/city-predictions?incompleteCityName=${incompleteCityName}`;

    const response: Response = await get(url);

    if (response.status === 200) {
        const predictions: Array<CityPrediction> = await response.json();

        return predictions;
    }

    return Promise.reject(await response.text());
}

export type Weather = {
    mainName: string,
    description: string,
    temperatureKelvin: number,
    temperatureKelvinFeelsLike: number,
    pressure: number,
    humidity: number,
    windSpeed: number
}

export const getTodayWeather = async (placeId: string): Promise<Weather> => {
    const url: string = `${baseUrl}weather/today?placeId=${placeId}`;

    const response: Response = await get(url);

    if (response.status === 200) {
        const weather: Weather = await response.json();

        return weather;
    }

    return Promise.reject(await response.text());
}

const get = (url: string): Promise<Response> => {
    const response: Promise<Response> = fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    return response;
}