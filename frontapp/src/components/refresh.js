import AuthService from "../services/auth.service"; 

const handleRefresh = async (user, navigate) => {  
  console.log("here");
  if (user != null) {
    await AuthService.loginWithRefreshToken(user.user.refreshToken).then(
      () => {
        window.location.reload();
      },
      () => {
        AuthService.logout();
        navigate("/");
        window.location.reload();
      }
    );
  } else {
    AuthService.logout();
    navigate("/");
    window.location.reload();
  }
};

export default handleRefresh;
