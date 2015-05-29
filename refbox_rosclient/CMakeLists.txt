cmake_minimum_required(VERSION 2.8.3)
project(refbox_rosclient)

## Find catkin macros and libraries
## if COMPONENTS list like find_package(catkin REQUIRED COMPONENTS xyz)
## is used, also find other catkin packages
find_package(catkin REQUIRED COMPONENTS
  roscpp
  std_msgs
)

###################################
## catkin specific configuration ##
###################################
## The catkin_package macro generates cmake config files for your package
## Declare things to be passed to dependent projects
## INCLUDE_DIRS: uncomment this if you package contains header files
## LIBRARIES: libraries you create in this project that dependent projects also need
## CATKIN_DEPENDS: catkin_packages dependent projects also need
## DEPENDS: system dependencies of this project that dependent projects also need
catkin_package(
#  INCLUDE_DIRS include
#  LIBRARIES test
#  CATKIN_DEPENDS roscpp std_msgs
#  DEPENDS system_lib
)

###########
## Build ##
###########

## Specify additional locations of header files
## Your package locations should be listed before other locations
# include_directories(include)
include_directories(
 	${catkin_INCLUDE_DIRS}
)

## Boost

set(Boost_USE_STATIC_LIBS   ON)
set(Boost_USE_MULTITHREADED ON)
set(Boost_USE_STATIC_RUNTIME ON)
FIND_PACKAGE( Boost 1.40 COMPONENTS program_options system filesystem regex thread date_time REQUIRED )

INCLUDE_DIRECTORIES(include ${catkin_INCLUDE_DIRS}
							${CMAKE_SOURCE_DIR}
							${Boost_INCLUDE_DIR}
					)

file(GLOB_RECURSE headers "src/*.h*")
file(GLOB_RECURSE sources "src/*.c*")

add_executable(refbox_rosclient ${sources} ${headers} )
target_link_libraries(refbox_rosclient ${catkin_LIBRARIES}
							${Boost_LIBRARIES}
			)
add_dependencies(refbox_rosclient refbox_rosclient_generate_messages_cpp)
