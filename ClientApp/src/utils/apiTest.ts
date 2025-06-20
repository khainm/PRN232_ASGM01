import newsService from '../services/newsService';

// Test function to check API responses
export const testApiEndpoints = async () => {
    console.log('=== Testing API Endpoints ===');
    
    try {
        console.log('1. Testing getActive():');
        const activeNews = await newsService.getActive();
        console.log('Active news response:', activeNews);
        console.log('Active news type:', typeof activeNews);
        console.log('Active news length:', activeNews?.length || 0);
        
        if (activeNews && activeNews.length > 0) {
            console.log('First active news item:', activeNews[0]);
            console.log('Property names:', Object.keys(activeNews[0]));
        }
        
        console.log('\n2. Testing getCategories():');
        const categories = await newsService.getCategories();
        console.log('Categories response:', categories);
        console.log('Categories type:', typeof categories);
        console.log('Categories length:', categories?.length || 0);
        
        if (categories && categories.length > 0) {
            console.log('First category item:', categories[0]);
            console.log('Property names:', Object.keys(categories[0]));
        }
        
        console.log('\n3. Testing getAll():');
        const allNews = await newsService.getAll();
        console.log('All news response:', allNews);
        console.log('All news length:', allNews?.length || 0);
        
    } catch (error) {
        console.error('Error testing API:', error);
    }
};

// Call this function from browser console
(window as any).testApiEndpoints = testApiEndpoints;
