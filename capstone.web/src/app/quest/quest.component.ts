import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QuestService } from '../services/quest.service';
import { PriorityService } from '../services/priority.service';
import { CategoryService } from '../services/category.service';
import { Quest } from '../models/quest';
import { Priority } from '../models/priority';
import { Category } from '../models/category';
import { MatOption } from '@angular/material/core';


@Component({
  selector: 'app-quest',
  templateUrl: './quest.component.html',
  styleUrl: './quest.component.scss'
})
export class QuestComponent implements OnInit {
  quest: Quest = { questId: 0, name: "", priorityId: 1, categoryId: 1, dueDate: new Date(), dateCreated: new Date(), isDeleted: false};
  priorities: Priority[] = [];
  categories: Category[] = [];
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private questService: QuestService,
    private priorityService: PriorityService,
    private categoryService: CategoryService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap?.get('id'); // Get quest id from route
    if (id) {
      // Fetch quest details if id exists
      this.questService.getQuest(parseInt(id)).subscribe((data) => {
        this.quest = data;
      });
    }

    this.priorityService.getPrioritys().subscribe((data) => {
      this.priorities = data;
      console.log(data);
    });

    this.categoryService.getCategories().subscribe((data) => {
      this.categories = data;
      console.log(data);
    });
  }

  saveQuest() {
    if (this.quest.questId) {
      this.questService.updateQuest(this.quest).subscribe(() => this.goBack());
    } 
    
    else {
      this.questService.addQuest(this.quest).subscribe(() => this.goBack());
    }
  }

  deleteQuest() {
    if (this.quest.questId) {
      this.questService.deleteQuest(this.quest.questId).subscribe(() => this.goBack());
    }
  }


  goBack() {
    this.router.navigate(["/quests"]);
  }
}
