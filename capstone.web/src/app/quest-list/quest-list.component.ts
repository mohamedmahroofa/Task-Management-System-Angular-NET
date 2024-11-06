import { Component , OnInit } from '@angular/core';
import { Quest } from '../models/quest';
import { QuestService } from '../services/quest.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-quest-list',
  templateUrl: './quest-list.component.html',
  styleUrl: './quest-list.component.css'
})
export class QuestListComponent implements OnInit{
  quests: Quest[] = []; // Array to hold list of quests
  displayedColumns: string[] = ['name', 'dueDate' , 'action','actiondelete'];
  
 
    constructor(
      private questService: QuestService,
      private router: Router
    ) {}
  
  ngOnInit(){
       // Fetch Quests on component initialization
       this.questService.getQuests().subscribe((data) => {
        this.quests = data;
      });
  }

  // Navigate to Quest details component
  viewQuest(id: number) {
    this.router.navigate(['/quest', id]);
  }
  deleteQuest(id: number) {
    if (confirm('Are you sure you want to delete this quest?')) {
      this.questService.deleteQuest(id).subscribe(() => {
        this.quests = this.quests.filter(quest => quest.questId !== id);
      });
    }
  }
  // Navigate to new Quest form
  addQuest() {
    this.router.navigate(['/quest']);
  }
  goBack() {
    this.router.navigate(['/']);
  }

 
}
