# LRUGenericCache

LRUGenericCache 
<p> 1. implements singleton pattern for cache </p>
<p> 2. it uses memoryCache (objectCache) under the hood </p>
<p> 3. allows caching of arbitrary object types </p>
<p> 4. implements LRU (Least Recently Used) eviction policy </p>
<p> 5. implements callback to inform user on evited item </p>
<p> 6. implements locking (separately for each key) using SemaphoreSlim </p>
