import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'stationFilter',
  standalone: true
})
export class StationFilterPipe implements PipeTransform {

  transform(stations: any[], searchText: string): any[] {
    if (!stations || !searchText) return stations;
    searchText = searchText.toLowerCase();
    return stations.filter(s => s.stationName.toLowerCase().includes(searchText));
  }

}
