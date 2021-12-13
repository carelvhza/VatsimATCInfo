# VatsimATCInfo

A website that displays useful information for a Vatsim controller.
It is hosted here: https://sky-net.co.za/vatsim-airport-info/

## Usage
![image](https://user-images.githubusercontent.com/20255491/129849132-762c3d43-04df-48d7-85f1-a848a26042a7.png)

On the top right you can enter an ICAO code for an aiport you would like to monitor. It will save this icao for every time you open it per browser.
This will load flight and weather information for the airport if it exists on the database and exists on Vatsim. 

## Info
### Vatsim
![image](https://user-images.githubusercontent.com/20255491/129849634-3b4fa02b-7186-43ff-9b79-44b3960ec0a6.png)

The page will display departures and arrivals for the airport, with information. Based on the state, it will show you if the aircraft is planning to depart at time, or is taxiying to the runway, or enroute with an ETA. You can hide aircraft that has departed to reduce clutter. If an aircraft has the airport for departure and arrival, it will assume they are doing circuits and display it as such. Hovering over a card will display the vatsim user's name. Prefiled flight plans will be shown above the active flights.

### Weather
![image](https://user-images.githubusercontent.com/20255491/129849669-06bfd489-e361-4757-89a7-a06c122cb55c.png)

A simple weather METAR decoder, that will decode the METAR and display its information on the UI. You can switch between Celsius and Fahrenheit.


