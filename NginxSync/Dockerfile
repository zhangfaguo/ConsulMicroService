# nginx Dockerfile
# Version 1.0
# author fendo
 
# Base images 基础镜像
#FROM centos:centos7
FROM hub.c.163.com/netease_comb/centos:7
 
#安装相关依赖
RUN yum -y update
RUN yum -y install  gcc gcc-c++ autoconf automake make
RUN yum -y install  zlib zlib-devel openssl* pcre* wget lua-devel
 
 
#ADD  获取url中的文件,放在当前目录下
ADD http://nginx.org/download/nginx-1.19.6.tar.gz /tmp/
ADD https://github.com/weibocom/nginx-upsync-module/archive/v2.1.3.tar.gz /tmp/

 
 
#切换目录
WORKDIR  /tmp  
 
#安装lua-nginx-module模块
RUN tar -xzvf v2.1.3.tar.gz
RUN cp -r nginx-upsync-module-2.1.3/ /usr/local/src/
 
RUN mkdir -p {/usr/local/nginx/logs,/var/lock}
volume "/etc/nginx/nginx.conf"
#编译安装Nginx
RUN useradd -M -s /sbin/nologin nginx
RUN tar -zxvf nginx-1.19.6.tar.gz
RUN mkdir -p /usr/local/nginx
RUN cd /tmp/nginx-1.19.6 \
    && ./configure --prefix=/etc/nginx --user=nginx --group=nginx \
    --conf-path=/etc/nginx/nginx.conf \
    --error-log-path=/var/log/nginx/error.log \
    --http-log-path=/var/log/nginx/access.log \
    --pid-path=/var/run/nginx.pid \
    --lock-path=/var/run/nginx.lock \
    --with-http_stub_status_module \
    --with-http_ssl_module \
    --with-http_sub_module \
    --add-module=/usr/local/src/nginx-upsync-module-2.1.3 \
    && make && make install
#参数说明
#--prefix 用于指定nginx编译后的安装目录
#--add-module 为添加的第三方模块，此次添加了fdfs的nginx模块
#--with..._module 表示启用的nginx模块，如此处启用了http_ssl_module模块    
 
RUN /etc/nginx/sbin/nginx -c /etc/nginx/nginx.conf
RUN ln -s /usr/local/nginx/sbin/* /usr/local/sbin/ &&\
	ln  -sf /dev/stdout /var/log/nginx/access.log && \     
    ln  -sf /dev/stderr /var/log/nginx/error.log      
 
#EXPOSE 映射端口
EXPOSE 80 443
 
#CMD 运行以下命令
#CMD ["nginx"]
CMD ["/etc/nginx/sbin/nginx","-g","daemon off;"]