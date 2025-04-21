import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'trainFilter',
  standalone: true
})
export class TrainFilterPipe implements PipeTransform {
  transform(trains: any[], searchText: string): any[] {
    if (!trains || !searchText) return trains;
    searchText = searchText.toLowerCase();

    return trains.filter(train =>
      train.trainName.toLowerCase().includes(searchText) ||
      train.trainID.toString().includes(searchText)
    );
  }
}

