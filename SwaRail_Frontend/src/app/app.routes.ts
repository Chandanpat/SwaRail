import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { UserDashboardComponent } from './components/user-dashboard/user-dashboard.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { AuthGuard } from './guards/auth.guard';
import { SearchTrainComponent } from './components/search-train/search-train.component';
import { BookTicketComponent } from './components/book-ticket/book-ticket.component';
import { MakePaymentComponent } from './components/make-payment/make-payment.component';
import { ShowTicketComponent } from './components/show-ticket/show-ticket.component';
import { MyBookingsComponent } from './components/my-bookings/my-bookings.component';
import { CancelBookingComponent } from './components/cancel-booking/cancel-booking.component';
import { ShowCancelledTicketComponent } from './components/show-cancelled-ticket/show-cancelled-ticket.component';
import { GetTrainsComponent } from './components/get-trains/get-trains.component';
import { AddTrainComponent } from './components/add-train/add-train.component';
import { EditTrainComponent } from './components/edit-train/edit-train.component';
import { ManageStationsComponent } from './components/manage-stations/manage-stations.component';
import { GetScheduleComponent } from './components/get-schedule/get-schedule.component';
import { AddScheduleComponent } from './components/add-schedule/add-schedule.component';
import { DeleteScheduleComponent } from './components/delete-schedule/delete-schedule.component';


export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'user-dashboard', component: UserDashboardComponent, canActivate: [AuthGuard], data: { expectedRole: 'User' }},
    { path: 'search-train', component: SearchTrainComponent, canActivate: [AuthGuard], data: { expectedRole: 'User' }},
    { path: 'book-ticket/:trainID/:scheduleID', component: BookTicketComponent, canActivate: [AuthGuard], data: { expectedRole: 'User' }},
    { path: 'make-payment/:reservationId', component: MakePaymentComponent, canActivate: [AuthGuard], data: { expectedRole: 'User' } },
    { path: 'show-ticket/:reservationId', component: ShowTicketComponent, canActivate: [AuthGuard], data: { expectedRole: 'User' } },
    { path: 'show-cancelled-ticket/:reservationId', component: ShowCancelledTicketComponent, canActivate: [AuthGuard], data: { expectedRole: 'User' } },
    { path: 'my-bookings', component: MyBookingsComponent,  canActivate: [AuthGuard], data: { expectedRole: 'User' } },
    { path: 'cancel-booking/:id', component: CancelBookingComponent,  canActivate: [AuthGuard], data: { expectedRole: 'User' }  },
    { path: 'admin-dashboard', component: AdminDashboardComponent, canActivate: [AuthGuard], data: { expectedRole: 'Admin' } },
    { path: 'get-trains', component: GetTrainsComponent, canActivate: [AuthGuard], data: { expectedRole: 'Admin' } },
    { path: 'add-train', component: AddTrainComponent, canActivate: [AuthGuard], data: { expectedRole: 'Admin' } },
    { path: 'edit-train/:trainId', component: EditTrainComponent, canActivate: [AuthGuard], data: { expectedRole: 'Admin' } },
    { path: 'manage-stations', component: ManageStationsComponent, canActivate: [AuthGuard], data: { expectedRole: 'Admin' } },
    { path: 'get-schedule', component: GetScheduleComponent, canActivate: [AuthGuard], data: { expectedRole: 'Admin' } },
    { path: 'add-schedule', component: AddScheduleComponent, canActivate: [AuthGuard], data: { expectedRole: 'Admin' } },
    { path: 'delete-schedule', component: DeleteScheduleComponent, canActivate: [AuthGuard], data: { expectedRole: 'Admin' } },
    { path: '**', redirectTo: 'login' }
];
