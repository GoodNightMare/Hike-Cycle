import { createContext, useContext, useState } from "react";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    return JSON.parse(localStorage.getItem("user"));
  });

  const login = (userData) => {
    localStorage.setItem("user", JSON.stringify(userData));
    setUser(userData);
  };

  const logout = () => {
    localStorage.removeItem("user");
    setUser(null);
  };

  // ✅ ฟังก์ชันอัปเดตโปรไฟล์ (เตรียมต่อ backend)
  const updateProfile = (updatedData) => {
    const newUser = { ...user, ...updatedData };

    console.log("📦 UPDATE USER:", newUser);

    // 🔜 backend
    // await api.put("/users/profile", updatedData)

    localStorage.setItem("user", JSON.stringify(newUser));
    setUser(newUser);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        login,
        logout,
        updateProfile, // ✅ เพิ่มตรงนี้
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
