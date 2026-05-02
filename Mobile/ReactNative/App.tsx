// Expo SDK 52+ entry point with React Query and native-stack navigation.
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { StatusBar } from 'expo-status-bar';

import { HomeScreen } from './HomeScreen';

export type RootStackParamList = {
  Home: undefined;
  Detail: { id: string };
};

const Stack = createNativeStackNavigator<RootStackParamList>();
const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <NavigationContainer>
        <StatusBar style="auto" />
        <Stack.Navigator>
          <Stack.Screen name="Home" component={HomeScreen} options={{ title: 'Posts' }} />
        </Stack.Navigator>
      </NavigationContainer>
    </QueryClientProvider>
  );
}
