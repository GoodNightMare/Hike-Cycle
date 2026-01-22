import { createContext, useContext, useEffect, useState } from "react";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // 🔁 โหลด user จาก localStorage ตอนเปิดเว็บ
  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    if (storedUser) {
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);

  // 🔐 login (เตรียมไว้ต่อ backend)
  const login = (userData) => {
    /**
     * userData ควรมีรูปแบบ:
     * {
     *   id,
     *   name,
     *   email,
     *   role: "user" | "admin" | "staff",
     *   token
     * }
     */
    localStorage.setItem("user", JSON.stringify(userData));
    setUser(userData);
  };

  // 🚪 logout
  const logout = () => {
    localStorage.removeItem("user");
    setUser(null);
  };

  // 🔄 update profile (ใช้กับหน้าโปรไฟล์)
  const updateUser = (updatedData) => {
    setUser((prev) => {
      const newUser = { ...prev, ...updatedData };
      localStorage.setItem("user", JSON.stringify(newUser));
      return newUser;
    });
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        login,
        logout,
        updateUser,
      }}
    >
      {!loading && children}
    </AuthContext.Provider>
  );
}

// 🪝 hook ใช้งาน
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth ต้องอยู่ภายใน AuthProvider");
  }
  return context;
};