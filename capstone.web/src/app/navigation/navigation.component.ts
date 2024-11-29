import { Component } from '@angular/core';

@Component({
    selector: 'app-navigation',
    templateUrl: './navigation.component.html',
    styleUrl: './navigation.component.css',
    standalone: false
})
export class NavigationComponent {

    currentDate: string = '';
    currentTime: string = '';
    dayOfWeek: string = '';
    intervalId: any;

    ngOnInit() {
        this.updateDateTime();
        this.intervalId = setInterval(() => {
          this.updateDateTime();
        }, 1000); // Update every second
      }

      ngOnDestroy() {
        if (this.intervalId) {
          clearInterval(this.intervalId); // Clear interval on component destruction
        }
      }
    
      updateDateTime() {
        const now = new Date();
        this.dayOfWeek = now.toLocaleString('en-US', { weekday: 'long' });
        this.currentDate = now.toLocaleDateString(); // Format: MM/DD/YYYY
        this.currentTime = now.toLocaleTimeString(); // Format: HH:MM:SS
      }

}
