import { Component, OnInit } from '@angular/core';
import { AccountService } from '../Service/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model:any ={}
  LoggedIn= false;

  constructor(private accountService:AccountService) { }

  ngOnInit(): void {
  }

  login (){
    console.log(this.model);
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
        this.LoggedIn = true;
      },
      error: error => console.log("falla en nav")
    });
  }

  logOut (){
    this.LoggedIn = false;
  }
}
