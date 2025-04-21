import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'scheduleFilter',
  standalone: true
})
export class ScheduleFilterPipe implements PipeTransform {

  transform(schedules: any[], filters: any, trains: any[]): any[] {
    if (!schedules) return [];

    return schedules.filter(schedule => {
      const scheduleIDMatch = !filters.scheduleID || schedule.scheduleID.toString().includes(filters.scheduleID);
      const journeyDateMatch = !filters.journeyDate || schedule.journeyDate.startsWith(filters.journeyDate);
      const trainNameMatch = !filters.trainName || 
        trains.find(t => t.trainID === schedule.trainID && t.trainName.toLowerCase().includes(filters.trainName.toLowerCase()));
      return scheduleIDMatch && journeyDateMatch && trainNameMatch;
    });
  }

}
