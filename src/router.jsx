import DefaultLayout from "./components/DefaultLayout";
import GuestLayout from "./components/GuestLayout";
import LoginPage from "./views/authenticationPages/LoginPage";
import OnboardingPage from "./views/authenticationPages/OnboardingPage";
import { createBrowserRouter } from "react-router-dom";
import UserDashboard from "./views/dashboard/UserDashboard";



const router = createBrowserRouter([
    {
        path: "/",
        element: <GuestLayout />,
        children: [
            {
                path: "login",
                element: <LoginPage />
            },
            {
                path: "register",
                element: <OnboardingPage />
            }
        ]
    }, 
    {
        path: "/default",
        element: <DefaultLayout />,
        children: [
            {
                path: "user",
                element: <UserDashboard />
            }
        ]
    }
]);

export default router;