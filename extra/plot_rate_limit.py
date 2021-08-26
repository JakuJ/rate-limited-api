import base64
import datetime
import time
from random import random

import matplotlib.pyplot as plt
from multiprocessing.pool import ThreadPool
import matplotlib.ticker as mtick
import numpy as np
import requests

# Ignore SSL errors
from urllib3.exceptions import InsecureRequestWarning

requests.packages.urllib3.disable_warnings(category=InsecureRequestWarning)


class User:
	def __init__(self, id: str, username: str, password: str):
		self.id = id
		self.username = username
		self.password = password


index = int(random() * 1000)


def register_user() -> User:
	global index
	data = {
		'username': f'username_{index}',
		'password': 'somepassword'
	}
	index += 1
	response = requests.post(url=f'https://localhost:5001/register', json=data, verify=False)
	if response.status_code == 200:
		return User(response.json()['clientID'], **data)
	else:
		print(response.json())


def spam_for(user: User, timespan: int, length: int):
	start = time.time()

	credentials = f'{user.username}:{user.password}'
	credentials_bytes = credentials.encode('ascii')
	base64_bytes = base64.b64encode(credentials_bytes)
	base64_credentials = base64_bytes.decode('ascii')

	headers = {
		'Authorization': f'Basic {base64_credentials}',
		'X-Client-ID'  : str(user.id)
	}

	reps = []
	total = 0
	while True:
		response = requests.get(url=f'https://localhost:5001/random?len={length}', headers=headers, verify=False)
		now = time.time()
		elapsed = now - start
		if elapsed > timespan:
			break
		if response.status_code == 200:
			total += length
			trip = response.elapsed.total_seconds() / 2
			reps.append((elapsed - trip, total))
			print('.', end='', flush=True)
		else:
			print('X', end='', flush=True)

	return reps


def run_one(length):
	user = register_user()
	data = spam_for(user, 45, length)

	xs = np.array([x for x, _ in data])
	ys = np.array([y for _, y in data])

	return xs, ys


legend = []


def run_all(num):
	steps = [32, 64, 128, 256, 512, 1024]
	num = min(num, len(steps))

	pool = ThreadPool(processes=num)
	results = pool.map_async(run_one, steps[:num])
	pool.close()
	pool.join()  # block until all threads exit
	xsys = results.get()

	longest_xs = sorted(xsys, key=lambda p: p[0][-1], reverse=True)[0][0]
	expected = 1024 + (longest_xs * (1024 / 10))
	plt.plot(longest_xs, expected)
	legend.append('Theoretical limit')

	for i, (xs, ys) in enumerate(xsys):
		plt.step(xs, ys, where='post')
		legend.append(f'User {i}, {steps[i]} bytes per request')


def main():
	for _ in range(5):
		register_user()  # skip the ones with custom limit in config

	num = 6
	run_all(num)

	plt.legend(legend)

	plt.ylabel('Bytes received')
	plt.xlabel('Seconds elapsed')
	plt.gcf().autofmt_xdate()

	plt.gca().xaxis.set_major_formatter(
		mtick.FuncFormatter(lambda pos, _: time.strftime("%S", time.localtime(pos)))
	)
	plt.tight_layout()
	plt.show()


if __name__ == "__main__":
	main()
