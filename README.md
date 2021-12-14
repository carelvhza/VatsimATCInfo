# VatsimATCInfo

A website that displays useful information for a Vatsim controller.
It is hosted here: https://sky-net.co.za/vatsim-airport-info/

## Usage
![image](https://user-images.githubusercontent.com/20255491/146038705-e8536aba-bd3c-415b-a181-a8b880cd7ae8.png)

On the top right you can enter an ICAO code for an aiport you would like to monitor. It will save this icao for every time you open it per browser.
This will load flight and weather information for the airport if it exists on the database and exists on Vatsim. 

## Info
### Vatsim
![image](https://user-images.githubusercontent.com/20255491/146038868-f1212843-4548-4372-a999-013627f0ce79.png)

The page will display departures and arrivals for the airport, with some information about each flight. Based on the state, it will show you if the aircraft is planning to depart at a time, or is taxiying to the runway, or enroute with an ETA. You can hide aircraft that has departed to reduce clutter. If an aircraft has the airport for departure and arrival, it will assume they are doing circuits and display it as such. Hovering over a card will display the vatsim user's name. Prefiled flight plans will be shown above the active flights.

### Weather
![image](https://user-images.githubusercontent.com/20255491/146039015-dd34572b-c7d4-4d1b-889d-925e62e163bd.png)

The weather panel will display parsed information from the Vatsim METAR for the airport. 
More than just parsing though, the panel will load and display the runway directions found at the airport. On first load, it will also automatically assign a direction for any of the available runways, based on the current wind direction. You can use the dropdowns to change the runways as needed for monitoring purposes while controlling.

### Extras
- If you turn it on in Settings, the background of the page will change depending on the current weather conditions at the airport. 
- Using the basic calculation of less than 5000m or less than 501ft cloud base, the panel will display weather an airport is IMC or VMC.
- In settings there is a option to use a dark background. (The panels wll remain light mode).

## Settings
![image](https://user-images.githubusercontent.com/20255491/146039523-14df5e1e-3180-4fcc-8b2d-02b0ae1259b9.png)

There are some basic settings, allowing you to scale the UI, show extra background effects, and more importantly, hide or show the panels in the UI that you want to see.
All of these settings are saved in local browser storage, so refreshing will keep your settings.

## Future Enhancements
- Add popup info boxes for the aircraft to allow clicking on a flight to see all of its details
- Popup for controllers, for the same reason
- Some form of integration with a vastim map app (maybe an embed?) to display a small 'radar' map area around the selected airport.

## Credit
- I used a great collection of flags in svg format, courtesy of Lipis. Find it at https://flagicons.lipis.dev/
- Uses **csharp-metar-decoder**, by "Safran Electronics & Defense - Cassiopée", github here: https://github.com/SafranCassiopee/csharp-metar-decoder
- I used the databases supplied by **OurAirports**, which is an invaluable resource for getting airport locations and runway information. Find them at https://ourairports.com/
- All the data comes from **Vatsim**, parsed from their data API's. 
