import time
from functools import wraps

def timing_decorator(func):
    @wraps(func)
    def wrapper(*args, **kwargs):
        start_time = time.time()
        result = func(*args, **kwargs)
        end_time = time.time()
        print(f"Function {func.__name__} took {(end_time - start_time) * 1000:.2f} ms to complete.")
        return result
    return wrapper

@timing_decorator
def example_function():
    # 模拟一个耗时的操作
    time.sleep(1)
    return "Result"

# 调用装饰过的函数
result = example_function()
print(result)
