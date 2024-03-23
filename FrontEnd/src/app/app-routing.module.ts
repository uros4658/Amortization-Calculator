import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ChartPageComponent } from './chart-page/chart-page.component';
import { HomeComponent } from './home';
import { AuthGuard } from './_helpers';
import { ShowDataComponent } from './showdata/showdata.component';
import { LoanFormComponent } from './loan-form/loan-form.component'; // Add this line

const accountModule = () => import('./account/account.module').then(x => x.AccountModule);
const usersModule = () => import('./users/users.module').then(x => x.UsersModule);

const routes: Routes = [
    { path: '', component: HomeComponent, canActivate: [AuthGuard] },
    { path: 'users', loadChildren: usersModule, canActivate: [AuthGuard] },
    { path: 'account', loadChildren: accountModule },
    { path: 'showdata', component: ShowDataComponent, canActivate: [AuthGuard] },
    { path: 'loan-form', component: LoanFormComponent, canActivate: [AuthGuard] }, // Add this line
    { path: 'chart', component: ChartPageComponent, canActivate: [AuthGuard]},

    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
